using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class GUI
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // ユーザーインターフェースと画面描画の処理を行うモジュール

        // MainのForm
        public static Form MainForm;
        public static bool IsFlashAvailable;

        // ADD START MARGE
        // GUIが新バージョンか
        public static bool NewGUIMode;
        // ADD END

        // マップ画面に表示できるマップのサイズ
        public static short MainWidth;
        public static short MainHeight;

        // マップ画面のサイズ（ピクセル）
        public static short MainPWidth;
        public static short MainPHeight;

        // マップのサイズ（ピクセル）
        public static short MapPWidth;
        public static short MapPHeight;

        // ＨＰ・ＥＮのゲージの幅（ピクセル）
        public const short GauageWidth = 88;

        // 現在マップウィンドウがマスク表示されているか
        public static bool ScreenIsMasked;
        // 現在マップウィンドウが保存されているか
        public static bool ScreenIsSaved;

        // 現在表示されているマップの座標
        public static short MapX;
        public static short MapY;

        // ドラッグ前のマップの座標
        public static short PrevMapX;
        public static short PrevMapY;

        // 最後に押されたマウスボタン
        public static short MouseButton;

        // 現在のマウスの座標
        public static float MouseX;
        public static float MouseY;

        // ドラッグ前のマウスの座標
        public static float PrevMouseX;
        public static float PrevMouseY;

        // カーソル位置自動変更前のマウスカーソルの座標
        private static short PrevCursorX;
        private static short PrevCursorY;
        // カーソル位置自動変更後のマウスカーソルの座標
        private static short NewCursorX;
        private static short NewCursorY;

        // 移動前のユニットの情報
        public static short PrevUnitX;
        public static short PrevUnitY;
        public static string PrevUnitArea;
        public static string PrevCommand;

        // PaintPictureで画像が描き込まれたか
        public static bool IsPictureDrawn;
        // PaintPictureで画像が描かれているか
        public static bool IsPictureVisible;
        // PaintPictureで描画した画像領域
        public static short PaintedAreaX1;
        public static short PaintedAreaY1;
        public static short PaintedAreaX2;
        public static short PaintedAreaY2;
        // カーソル画像が表示されているか
        public static bool IsCursorVisible;
        // 背景色
        public static int BGColor;

        // 画像バッファ管理用変数
        private static int PicBufDateCount;
        private static int[] PicBufDate;
        private static int[] PicBufSize;
        private static string[] PicBufFname;
        private static string[] PicBufOption;
        private static string[] PicBufOption2;
        private static short[] PicBufDW;
        private static short[] PicBufDH;
        private static short[] PicBufSX;
        private static short[] PicBufSY;
        private static short[] PicBufSW;
        private static short[] PicBufSH;
        private static bool[] PicBufIsMask;


        // GUIから入力可能かどうか
        public static bool IsGUILocked;

        // リストボックス内で表示位置
        public static short TopItem;

        // メッセージウインドウにに関する情報
        private static string DisplayedPilot;
        private static string DisplayMode;
        private static Unit RightUnit;
        private static Unit LeftUnit;
        private static double RightUnitHPRatio;
        private static double LeftUnitHPRatio;
        private static double RightUnitENRatio;
        private static double LeftUnitENRatio;
        public static bool MessageWindowIsOut;

        // メッセージウィンドウの状態を保持するための変数
        private static bool IsMessageFormVisible;
        private static Unit SavedLeftUnit;
        private static Unit SavedRightUnit;

        // フォームがクリックされたか
        public static bool IsFormClicked;
        // フォームがモーダルか
        public static bool IsMordal;

        // メッセージ表示のウェイト
        public static int MessageWait;

        // メッセージが自働送りかどうか
        public static bool AutoMessageMode;

        // PaintStringの中央表示の設定
        public static bool HCentering;
        public static bool VCentering;
        // PaintStringの書きこみが背景に行われるかどうか
        public static bool PermanentStringMode;
        // PaintStringの書きこみが持続性かどうか
        public static bool KeepStringMode;


        // ListBox用変数
        public static bool[] ListItemFlag;
        public static string[] ListItemComment;
        public static string[] ListItemID;
        public static short MaxListItem;


        // API関数の定義

        [DllImport("gdi32")]
        static extern int BitBlt(int hDestDC, int X, int Y, int nWidth, int nHeight, int hSrcDC, int xsrc, int ysrc, int dwRop);
        [DllImport("gdi32")]
        static extern int StretchBlt(int hDestDC, int X, int Y, int nWidth, int nHeight, int hSrcDC, int xsrc, int ysrc, int nSrcWidth, int nSrcHeight, int dwRop);
        [DllImport("gdi32")]
        static extern int PatBlt(int hDC, int X, int Y, int nWidth, int nHeight, int dwRop);

        public const int BLACKNESS = 0x42;
        public const int DSTINVERT = 0x550009;
        public const int MERGECOPY = 0xC000CA;
        public const int MERGEPAINT = 0xBB0226;
        public const int NOTSRCCOPY = 0x330008;
        public const int NOTSRCERASE = 0x1100A6;
        public const int PATCOPY = 0xF00021;
        public const int PATINVERT = 0x5A0049;
        public const int PATPAINT = 0xFB0A09;
        public const int SRCAND = 0x8800C6;
        public const int SRCCOPY = 0xCC0020;
        public const int SRCERASE = 0x440328;
        public const int SRCINVERT = 0x660046;
        public const int SRCPAINT = 0xEE0086;
        public const int WHITENESS = 0xFF0062;
        // ADD START 240a
        public const int STATUSBACK = 0xC0C0C0;
        // ADD START 240a

        // StretchBltのモード設定を行う
        [DllImport("gdi32")]
        static extern int GetStretchBltMode(int hDC);
        [DllImport("gdi32")]
        static extern int SetStretchBltMode(int hDC, int nStretchMode);

        public const short STRETCH_ANDSCANS = 1;
        public const short STRETCH_ORSCANS = 2;
        public const short STRETCH_DELETESCANS = 3;
        public const short STRETCH_HALFTONE = 4;

        // 透過描画
        [DllImport("msimg32.dll")]
        static extern int TransparentBlt(int hDC, int X, int Y, int nWidth, int nHeight, int hSrcDC, int xsrc, int ysrc, int nSrcWidth, int nSrcHeight, int crTransparent);

        // ウィンドウ位置の設定
        [DllImport("user32")]
        static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int wFlags);

        public const short SW_SHOWNA = 8; // 非アクティブで表示

        // フォームをアクティブにしないで表示
        [DllImport("user32")]
        static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("kernel32")]
        static extern void Sleep(int dwMilliseconds);

        // カーソル位置取得
        // UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("user32")]
        static extern int GetCursorPos(ref POINTAPI lpPoint);

        // ポイント構造体
        public struct POINTAPI
        {
            public int X;
            public int Y;
        }

        // カーソル位置設定
        [DllImport("user32")]
        static extern int SetCursorPos(int X, int Y);

        // キーの情報を得る
        [DllImport("user32")]
        static extern short GetAsyncKeyState(int vKey);

        public static int RButtonID;
        public static int LButtonID;

        // システムメトリックスを取得するAPI
        [DllImport("user32")]
        static extern int GetSystemMetrics(int nIndex);

        public const short SM_SWAPBUTTON = 23; // 左右のボタンが交換されているか否か

        // 現在アクティブなウィンドウを取得するAPI
        [DllImport("user32")]
        public static extern int GetForegroundWindow();

        // 直線を描画するためのAPI
        // UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("gdi32")]
        static extern int MoveToEx(int hDC, int X, int Y, ref POINTAPI lpPoint);
        [DllImport("gdi32")]
        static extern int LineTo(int hDC, int X, int Y);

        // 多角形を描画するAPI
        // UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("gdi32.dll")]
        public static extern int Polygon(int hDC, ref POINTAPI lpPoint, int nCount);


        // ディスプレイの設定を参照するAPI
        public struct DEVMODE
        {
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(32)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(32)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] dmFormName;
            public short dmUnusedPadding;
            public short dmBitsPerPixel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        // UPGRADE_WARNING: 構造体 DEVMODE に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("user32.dll", EntryPoint = "EnumDisplaySettingsA")]
        public static extern int EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        public const short ENUM_CURRENT_SETTINGS = -1;

        // ディスプレイの設定を変更するためのAPI
        [DllImport("user32.dll", EntryPoint = "ChangeDisplaySettingsA")]
        public static extern int ChangeDisplaySettings(ref Any lpDevMode, int dwFlags);

        public const int CDS_UPDATEREGISTRY = 0x1;
        public const int CDS_TEST = 0x2;
        public const int CDS_FULLSCREEN = 0x4;
        public const short DISP_CHANGE_SUCCESSFUL = 0;
        public const short DISP_CHANGE_RESTART = 1;

        // デバイスの設定を参照するためのAPI
        [DllImport("gdi32")]
        public static extern int GetDeviceCaps(int hDC, int nIndex);

        // ピクセル当たりのカラービット数
        private const short BITSPIXEL = 12;


        // システムパラメータを変更するためのAPI
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        static extern int SetSystemParametersInfo(int uiAction, int uiParam, int pvParam, int fWinIni);
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        static extern int GetSystemParametersInfo(int uiAction, int uiParam, ref int pvParam, int fWinIni);

        // フォントのスムージング処理関連の定数
        public const short SPI_GETFONTSMOOTHING = 74;
        public const short SPI_SETFONTSMOOTHING = 75;

        // ユーザープロファイルの更新を指定
        public const int SPIF_UPDATEINIFILE = 0x1;
        // すべてのトップレベルウィンドウに変更を通知
        public const int SPIF_SENDWININICHANGE = 0x2;


        // メインウィンドウのロードとFlashの登録を行う
        public static void LoadMainFormAndRegisterFlash()
        {
            object WSHShell;
            ;

            // シェルからregsvr32.exeを利用して、起動ごとにSRC.exeと同じパスにある
            // FlashControl.ocxを再登録する。
            WSHShell = Interaction.CreateObject("WScript.Shell");
            // UPGRADE_WARNING: オブジェクト WSHShell.Run の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            WSHShell.Run("regsvr32.exe /s \"" + SRC.AppPath + "FlashControl.ocx\"", 0, true);
            // UPGRADE_NOTE: オブジェクト WSHShell をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            WSHShell = null;

            Load(My.MyProject.Forms.frmMain);
            MainForm = My.MyProject.Forms.frmMain;
            IsFlashAvailable = true;
            return;
            ErrorHandler:
            ;


            // Flashが使えないのでFlash無しのメインウィンドウを使用する
            Load(My.MyProject.Forms.frmSafeMain);
            MainForm = My.MyProject.Forms.frmSafeMain;
        }

        // 各ウィンドウをロード
        // ただしメインウィンドウはあらかじめLoadMainFormAndRegisterFlashでロードしておくこと
        public static void LoadForms()
        {
            short X, Y;

            Load(My.MyProject.Forms.frmToolTip);
            Load(My.MyProject.Forms.frmMessage);
            Load(My.MyProject.Forms.frmListBox);
            LockGUI();
            Commands.CommandState = "ユニット選択";

            // マップ画面に表示できるマップのサイズ
            string argini_section1 = "Option";
            string argini_entry1 = "NewGUI";
            switch (Strings.LCase(GeneralLib.ReadIni(ref argini_section1, ref argini_entry1)) ?? "")
            {
                case "on":
                    {
                        // MOD START MARGE
                        NewGUIMode = true;
                        // MOD END MARGE
                        MainWidth = 20;
                        break;
                    }

                case "off":
                    {
                        MainWidth = 15;
                        break;
                    }

                default:
                    {
                        MainWidth = 15;
                        string argini_section = "Option";
                        string argini_entry = "NewGUI";
                        string argini_data = "Off";
                        GeneralLib.WriteIni(ref argini_section, ref argini_entry, ref argini_data);
                        break;
                    }
            }
            // ADD START MARGE
            // Optionで定義されていればそちらを優先する
            string argoname = "新ＧＵＩ";
            if (Expression.IsOptionDefined(ref argoname))
            {
                NewGUIMode = true;
                MainWidth = 20;
            }
            // ADD END MARGE
            MainHeight = 15;

            // マップ画面のサイズ（ピクセル）
            MainPWidth = (short)(MainWidth * 32);
            MainPHeight = (short)(MainHeight * 32);
            {
                var withBlock = MainForm;
                // メインウィンドウの位置＆サイズを設定
                X = (short)SrcFormatter.TwipsPerPixelX();
                Y = (short)SrcFormatter.TwipsPerPixelY();
                // MOD START MARGE
                // If MainWidth = 15 Then
                if (!NewGUIMode)
                {
                    // MOD END MARGE
                    withBlock.Width = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(withBlock.Width) - SrcFormatter.PixelsToTwipsX(withBlock.ClientRectangle.Width) * X + (MainPWidth + 24 + 225 + 4) * X);
                    withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - SrcFormatter.PixelsToTwipsY(withBlock.ClientRectangle.Height) * Y + (MainPHeight + 24) * Y);
                }
                else
                {
                    withBlock.Width = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(withBlock.Width) - SrcFormatter.PixelsToTwipsX(withBlock.ClientRectangle.Width) * X + MainPWidth * X);
                    withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - SrcFormatter.PixelsToTwipsY(withBlock.ClientRectangle.Height) * Y + MainPHeight * Y);
                }

                withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
                withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);

                // スクロールバーの位置を設定
                // MOD START MARGE
                // If MainWidth = 15 Then
                if (!NewGUIMode)
                {
                    // MOD END MARGE
                    withBlock.VScroll.Move(MainPWidth + 4, 4, 16, MainPWidth);
                    withBlock.HScroll.Move(4, MainPHeight + 4, MainPWidth, 16);
                }
                else
                {
                    withBlock.VScroll.Visible = false;
                    withBlock.HScroll.Visible = false;
                }

                // ステータスウィンドウを設置
                // MOD START MARGE
                // If MainWidth = 15 Then
                // .picFace.Move MainPWidth + 24, 4
                // .picPilotStatus.Move MainPWidth + 24 + 68 + 4, 4, 155, 72
                // .picUnitStatus.Move MainPWidth + 24, 4 + 68 + 4, _
                // '                225 + 5, MainPHeight - 64 + 16
                // Else
                // .picUnitStatus.Move MainPWidth - 230 - 10, 10, 230, MainPHeight - 20
                // .picUnitStatus.Visible = False
                // .picPilotStatus.Visible = False
                // .picFace.Visible = False
                // End If
                if (NewGUIMode)
                {
                    withBlock.picUnitStatus.Move(MainPWidth - 230 - 10, 10, 230, MainPHeight - 20);
                    withBlock.picUnitStatus.Visible = false;
                    withBlock.picPilotStatus.Visible = false;
                    withBlock.picFace.Visible = false;
                    Status.StatusWindowBackBolor = STATUSBACK;
                    Status.StatusWindowFrameColor = STATUSBACK;
                    Status.StatusWindowFrameWidth = 1;
                    withBlock.picUnitStatus.BackColor = Status.StatusWindowBackBolor;
                    Status.StatusFontColorAbilityName = Information.RGB(0, 0, 150);
                    Status.StatusFontColorAbilityEnable = ColorTranslator.ToOle(Color.Blue);
                    Status.StatusFontColorAbilityDisable = Information.RGB(150, 0, 0);
                    Status.StatusFontColorNormalString = ColorTranslator.ToOle(Color.Black);
                }
                else
                {
                    withBlock.picFace.Move(MainPWidth + 24, 4);
                    withBlock.picPilotStatus.Move(MainPWidth + 24 + 68 + 4, 4, 155, 72);
                    withBlock.picUnitStatus.Move(MainPWidth + 24, 4 + 68 + 4, 225 + 5, MainPHeight - 64 + 16);
                }
                // MOD END MARGE

                // マップウィンドウのサイズを設定
                // MOD START MARGE
                // If MainWidth = 15 Then
                if (!NewGUIMode)
                {
                    // MOD END MARGE
                    withBlock.picMain(0).Move(4, 4, MainPWidth, MainPHeight);
                    withBlock.picMain(1).Move(4, 4, MainPWidth, MainPHeight);
                }
                else
                {
                    withBlock.picMain(0).Move(0, 0, MainPWidth, MainPHeight);
                    withBlock.picMain(1).Move(0, 0, MainPWidth, MainPHeight);
                }
            }
        }

        // ADD START MARGE
        // Optionによる新ＧＵＩが有効かどうかを再設定する
        public static void SetNewGUIMode()
        {
            // Optionで定義されているのにNewGUIModeがfalseの場合、LoadFormsを呼ぶ
            string argoname = "新ＧＵＩ";
            if (Expression.IsOptionDefined(ref argoname) & !NewGUIMode)
            {
                LoadForms();
            }
        }
        // ADD  END  MARGE

        // === メッセージウィンドウに関する処理 ===

        // メッセージウィンドウを開く
        // 戦闘メッセージ画面など、ユニット表示を行う場合は u1, u2 に指定
        public static void OpenMessageForm([Optional, DefaultParameterValue(null)] ref Unit u1, [Optional, DefaultParameterValue(null)] ref Unit u2)
        {
            short tppx, tppy;
            int ret;

            // UPGRADE_NOTE: オブジェクト RightUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            RightUnit = null;
            // UPGRADE_NOTE: オブジェクト LeftUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            LeftUnit = null;

            tppx = (short)SrcFormatter.TwipsPerPixelX();
            tppy = (short)SrcFormatter.TwipsPerPixelY();

            Load(My.MyProject.Forms.frmMessage);
            {
                var withBlock = My.MyProject.Forms.frmMessage;
                // ユニット表示を伴う場合はキャプションから「(自動送り)」を削除
                if (u1 is object)
                {
                    if (withBlock.Text == "メッセージ (自動送り)")
                    {
                        withBlock.Text = "メッセージ";
                    }
                }

                // メッセージウィンドウを強制的に最小化解除
                if (withBlock.WindowState != FormWindowState.Normal)
                {
                    withBlock.WindowState = FormWindowState.Normal;
                    withBlock.Show();
                    withBlock.Activate();
                }

                if (u1 is null)
                {
                    // ユニット表示なし
                    withBlock.labHP1.Visible = false;
                    withBlock.labHP2.Visible = false;
                    withBlock.labEN1.Visible = false;
                    withBlock.labEN2.Visible = false;
                    withBlock.picHP1.Visible = false;
                    withBlock.picHP2.Visible = false;
                    withBlock.picEN1.Visible = false;
                    withBlock.picEN2.Visible = false;
                    withBlock.txtHP1.Visible = false;
                    withBlock.txtHP2.Visible = false;
                    withBlock.txtEN1.Visible = false;
                    withBlock.txtEN2.Visible = false;
                    withBlock.picUnit1.Visible = false;
                    withBlock.picUnit2.Visible = false;
                    withBlock.Width = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(withBlock.Width) - withBlock.ClientRectangle.Width * tppx + 508 * tppx);
                    withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - withBlock.ClientRectangle.Height * tppy + 84 * tppy);
                    withBlock.picFace.Top = 8;
                    withBlock.picFace.Left = 8;
                    withBlock.picMessage.Top = 7;
                    withBlock.picMessage.Left = 84;
                }
                else if (u2 is null)
                {
                    // ユニット表示１体のみ
                    if (u1.Party == "味方" | u1.Party == "ＮＰＣ")
                    {
                        withBlock.labHP1.Visible = false;
                        withBlock.labEN1.Visible = false;
                        withBlock.picHP1.Visible = false;
                        withBlock.picEN1.Visible = false;
                        withBlock.txtHP1.Visible = false;
                        withBlock.txtEN1.Visible = false;
                        withBlock.picUnit1.Visible = false;
                        withBlock.labHP2.Visible = true;
                        withBlock.labEN2.Visible = true;
                        withBlock.picHP2.Visible = true;
                        withBlock.picEN2.Visible = true;
                        withBlock.txtHP2.Visible = true;
                        withBlock.txtEN2.Visible = true;
                        withBlock.picUnit2.Visible = true;
                    }
                    else
                    {
                        withBlock.labHP1.Visible = true;
                        withBlock.labEN1.Visible = true;
                        withBlock.picHP1.Visible = true;
                        withBlock.picEN1.Visible = true;
                        withBlock.txtHP1.Visible = true;
                        withBlock.txtEN1.Visible = true;
                        withBlock.picUnit1.Visible = true;
                        withBlock.labHP2.Visible = false;
                        withBlock.labEN2.Visible = false;
                        withBlock.picHP2.Visible = false;
                        withBlock.picEN2.Visible = false;
                        withBlock.txtHP2.Visible = false;
                        withBlock.txtEN2.Visible = false;
                        withBlock.picUnit2.Visible = false;
                    }

                    object argu21 = null;
                    UpdateMessageForm(ref u1, u2: ref argu21);
                    withBlock.Width = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(withBlock.Width) - withBlock.ClientRectangle.Width * tppx + 508 * tppx);
                    withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - withBlock.ClientRectangle.Height * tppy + 118 * tppy);
                    withBlock.picFace.Top = 42;
                    withBlock.picFace.Left = 8;
                    withBlock.picMessage.Top = 41;
                    withBlock.picMessage.Left = 84;
                }
                else
                {
                    // ユニットを２体表示
                    withBlock.labHP1.Visible = true;
                    withBlock.labHP2.Visible = true;
                    withBlock.labEN1.Visible = true;
                    withBlock.labEN2.Visible = true;
                    withBlock.picHP1.Visible = true;
                    withBlock.picHP2.Visible = true;
                    withBlock.picEN1.Visible = true;
                    withBlock.picEN2.Visible = true;
                    withBlock.txtHP1.Visible = true;
                    withBlock.txtHP2.Visible = true;
                    withBlock.txtEN1.Visible = true;
                    withBlock.txtEN2.Visible = true;
                    withBlock.picUnit1.Visible = true;
                    withBlock.picUnit2.Visible = true;
                    object argu2 = u2;
                    UpdateMessageForm(ref u1, ref argu2);
                    withBlock.Width = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(withBlock.Width) - withBlock.ClientRectangle.Width * tppx + 508 * tppx);
                    withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - withBlock.ClientRectangle.Height * tppy + 118 * tppy);
                    withBlock.picFace.Top = 42;
                    withBlock.picFace.Left = 8;
                    withBlock.picMessage.Top = 41;
                    withBlock.picMessage.Left = 84;
                }

                // メッセージウィンドウの位置設定
                if (MainForm.Visible & !((int)MainForm.WindowState == 1))
                {
                    // メインウィンドウが表示されていればメインウィンドウの下端に合わせて表示
                    if (!My.MyProject.Forms.frmMessage.Visible)
                    {
                        if (MainWidth == 15)
                        {
                            withBlock.Left = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(MainForm.Left));
                        }
                        else
                        {
                            withBlock.Left = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(MainForm.Left) + (long)(SrcFormatter.PixelsToTwipsX(MainForm.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2L);
                        }

                        if (MessageWindowIsOut)
                        {
                            withBlock.Top = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(MainForm.Top) + SrcFormatter.PixelsToTwipsY(MainForm.Height) - 350d);
                        }
                        else
                        {
                            withBlock.Top = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(MainForm.Top) + SrcFormatter.PixelsToTwipsY(MainForm.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height));
                        }
                    }
                }
                else
                {
                    // メインウィンドウが表示されていない場合は画面中央に表示
                    withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);
                }

                // ウィンドウをクリアしておく
                withBlock.picFace.Image = Image.FromFile("");
                DisplayedPilot = "";
                withBlock.picMessage.Cls();

                // ウィンドウを表示
                withBlock.Show();

                // 常に手前に表示する
                ret = SetWindowPos(withBlock.Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
            }

            Application.DoEvents();
        }

        // メッセージウィンドウを閉じる
        public static void CloseMessageForm()
        {
            if (!My.MyProject.Forms.frmMessage.Visible)
            {
                return;
            }

            My.MyProject.Forms.frmMessage.Hide();
            Application.DoEvents();
        }

        // メッセージウィンドウをクリア
        public static void ClearMessageForm()
        {
            {
                var withBlock = My.MyProject.Forms.frmMessage;
                withBlock.picFace.Image = Image.FromFile("");
                withBlock.picMessage.Cls();
            }

            DisplayedPilot = "";
            // UPGRADE_NOTE: オブジェクト LeftUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            LeftUnit = null;
            // UPGRADE_NOTE: オブジェクト RightUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            RightUnit = null;
            Application.DoEvents();
        }

        // メッセージウィンドウに表示しているユニット情報を更新
        public static void UpdateMessageForm(ref Unit u1, [Optional, DefaultParameterValue(null)] ref object u2)
        {
            Unit lu, ru;
            int ret;
            var i = default(short);
            string buf;
            var num = default(short);
            int tmp;
            {
                var withBlock = My.MyProject.Forms.frmMessage;
                // ウィンドウにユニット情報が表示されていない場合はそのまま終了
                if (withBlock.Visible)
                {
                    if (!withBlock.picUnit1.Visible & !withBlock.picUnit2.Visible)
                    {
                        return;
                    }
                }

                // luを左に表示するユニット、ruを右に表示するユニットに設定
                // UPGRADE_NOTE: IsMissing() は IsNothing() に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="8AE1CB93-37AB-439A-A4FF-BE3B6760BB23"' をクリックしてください。
                if (Information.IsNothing(u2))
                {
                    // １体のユニットのみ表示
                    if (u1.Party == "味方" | u1.Party == "ＮＰＣ")
                    {
                        // UPGRADE_NOTE: オブジェクト lu をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        lu = null;
                        ru = u1;
                    }
                    else
                    {
                        lu = u1;
                        // UPGRADE_NOTE: オブジェクト ru をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        ru = null;
                    }
                }
                else if (u2 is null)
                {
                    // 反射攻撃
                    // 前回表示されたユニットをそのまま使用
                    lu = LeftUnit;
                    ru = RightUnit;
                }
                else if ((ReferenceEquals(u2, LeftUnit) | ReferenceEquals(u1, RightUnit)) & !ReferenceEquals(LeftUnit, RightUnit))
                {
                    lu = (Unit)u2;
                    ru = u1;
                }
                else
                {
                    lu = u1;
                    ru = (Unit)u2;
                }

                // 現在表示されている順番に応じてユニットの入れ替え
                if (ReferenceEquals(lu, RightUnit) & ReferenceEquals(ru, LeftUnit) & !ReferenceEquals(LeftUnit, RightUnit))
                {
                    lu = LeftUnit;
                    ru = RightUnit;
                }

                // 表示するユニットのＧＵＩ部品を表示
                if (lu is object)
                {
                    if (!withBlock.labHP1.Visible)
                    {
                        withBlock.labHP1.Visible = true;
                        withBlock.labEN1.Visible = true;
                        withBlock.picHP1.Visible = true;
                        withBlock.picEN1.Visible = true;
                        withBlock.txtHP1.Visible = true;
                        withBlock.txtEN1.Visible = true;
                        withBlock.picUnit1.Visible = true;
                    }
                }

                if (ru is object)
                {
                    if (!withBlock.labHP2.Visible)
                    {
                        withBlock.labHP2.Visible = true;
                        withBlock.labEN2.Visible = true;
                        withBlock.picHP2.Visible = true;
                        withBlock.picEN2.Visible = true;
                        withBlock.txtHP2.Visible = true;
                        withBlock.txtEN2.Visible = true;
                        withBlock.picUnit2.Visible = true;
                    }
                }

                // 未表示のユニットを表示する
                if (lu is object & !ReferenceEquals(lu, LeftUnit))
                {
                    // 左のユニットが未表示なので表示する

                    // ユニット画像
                    if (lu.BitmapID > 0)
                    {
                        if (string.IsNullOrEmpty(Map.MapDrawMode))
                        {
                            ret = BitBlt(withBlock.picUnit1.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * ((int)lu.BitmapID % 15), 96 * ((int)lu.BitmapID / 15), SRCCOPY);
                        }
                        else
                        {
                            var argpic = withBlock.picUnit1;
                            string argfname = "";
                            LoadUnitBitmap(ref lu, ref argpic, 0, 0, true, fname: ref argfname);
                            withBlock.picUnit1 = argpic;
                        }
                    }
                    else
                    {
                        // 非表示のユニットの場合はユニットのいる地形タイルを表示
                        ret = BitBlt(withBlock.picUnit1.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * ((int)lu.x - 1), 32 * ((int)lu.y - 1), SRCCOPY);
                    }

                    withBlock.picUnit1.Refresh();

                    // ＨＰ名称
                    object argIndex1 = "データ不明";
                    if (lu.IsConditionSatisfied(ref argIndex1))
                    {
                        string argtname = "HP";
                        Unit argu = null;
                        withBlock.labHP1.Text = Expression.Term(ref argtname, u: ref argu);
                    }
                    else
                    {
                        string argtname1 = "HP";
                        withBlock.labHP1.Text = Expression.Term(ref argtname1, ref lu);
                    }

                    // ＨＰ数値
                    object argIndex2 = "データ不明";
                    if (lu.IsConditionSatisfied(ref argIndex2))
                    {
                        withBlock.txtHP1.Text = "?????/?????";
                    }
                    else
                    {
                        if (lu.HP < 100000)
                        {
                            string argbuf = SrcFormatter.Format(lu.HP);
                            buf = GeneralLib.LeftPaddedString(ref argbuf, (short)GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxHP)), 5));
                        }
                        else
                        {
                            buf = "?????";
                        }

                        if (lu.MaxHP < 100000)
                        {
                            buf = buf + "/" + SrcFormatter.Format(lu.MaxHP);
                        }
                        else
                        {
                            buf = buf + "/?????";
                        }

                        withBlock.txtHP1.Text = buf;
                    }

                    // ＨＰゲージ
                    withBlock.picHP1.Cls();
                    if (lu.HP > 0 | i < num)
                    {
                        withBlock.picHP1.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    }

                    // ＥＮ名称
                    object argIndex3 = "データ不明";
                    if (lu.IsConditionSatisfied(ref argIndex3))
                    {
                        string argtname2 = "EN";
                        Unit argu1 = null;
                        withBlock.labEN1.Text = Expression.Term(ref argtname2, u: ref argu1);
                    }
                    else
                    {
                        string argtname3 = "EN";
                        withBlock.labEN1.Text = Expression.Term(ref argtname3, ref lu);
                    }

                    // ＥＮ数値
                    object argIndex4 = "データ不明";
                    if (lu.IsConditionSatisfied(ref argIndex4))
                    {
                        withBlock.txtEN1.Text = "???/???";
                    }
                    else
                    {
                        if (lu.EN < 1000)
                        {
                            string argbuf1 = SrcFormatter.Format(lu.EN);
                            buf = GeneralLib.LeftPaddedString(ref argbuf1, (short)GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxEN)), 3));
                        }
                        else
                        {
                            buf = "???";
                        }

                        if (lu.MaxEN < 1000)
                        {
                            buf = buf + "/" + SrcFormatter.Format(lu.MaxEN);
                        }
                        else
                        {
                            buf = buf + "/???";
                        }

                        withBlock.txtEN1.Text = buf;
                    }

                    // ＥＮゲージ
                    withBlock.picEN1.Cls();
                    if (lu.EN > 0 | i < num)
                    {
                        withBlock.picEN1.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    }

                    // 表示内容を記録
                    LeftUnit = lu;
                    LeftUnitHPRatio = lu.HP / (double)lu.MaxHP;
                    LeftUnitENRatio = lu.EN / (double)lu.MaxEN;
                }

                if (ru is object & !ReferenceEquals(RightUnit, ru))
                {
                    // 右のユニットが未表示なので表示する

                    // ユニット画像
                    if (ru.BitmapID > 0)
                    {
                        if (string.IsNullOrEmpty(Map.MapDrawMode))
                        {
                            ret = BitBlt(withBlock.picUnit2.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * ((int)ru.BitmapID % 15), 96 * ((int)ru.BitmapID / 15), SRCCOPY);
                        }
                        else
                        {
                            var argpic1 = withBlock.picUnit2;
                            string argfname1 = "";
                            LoadUnitBitmap(ref ru, ref argpic1, 0, 0, true, fname: ref argfname1);
                            withBlock.picUnit2 = argpic1;
                        }
                    }
                    else
                    {
                        // 非表示のユニットの場合はユニットのいる地形タイルを表示
                        ret = BitBlt(withBlock.picUnit2.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * ((int)ru.x - 1), 32 * ((int)ru.y - 1), SRCCOPY);
                    }

                    withBlock.picUnit2.Refresh();

                    // ＨＰ数値
                    object argIndex5 = "データ不明";
                    if (ru.IsConditionSatisfied(ref argIndex5))
                    {
                        string argtname4 = "HP";
                        Unit argu2 = null;
                        withBlock.labHP2.Text = Expression.Term(ref argtname4, u: ref argu2);
                    }
                    else
                    {
                        string argtname5 = "HP";
                        withBlock.labHP2.Text = Expression.Term(ref argtname5, ref ru);
                    }

                    // ＨＰ数値
                    object argIndex6 = "データ不明";
                    if (ru.IsConditionSatisfied(ref argIndex6))
                    {
                        withBlock.txtHP2.Text = "?????/?????";
                    }
                    else
                    {
                        if (ru.HP < 100000)
                        {
                            string argbuf2 = SrcFormatter.Format(ru.HP);
                            buf = GeneralLib.LeftPaddedString(ref argbuf2, (short)GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxHP)), 5));
                        }
                        else
                        {
                            buf = "?????";
                        }

                        if (ru.MaxHP < 100000)
                        {
                            buf = buf + "/" + SrcFormatter.Format(ru.MaxHP);
                        }
                        else
                        {
                            buf = buf + "/?????";
                        }

                        withBlock.txtHP2.Text = buf;
                    }

                    // ＨＰゲージ
                    withBlock.picHP2.Cls();
                    if (ru.HP > 0 | i < num)
                    {
                        withBlock.picHP2.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    }

                    // ＥＮ名称
                    object argIndex7 = "データ不明";
                    if (ru.IsConditionSatisfied(ref argIndex7))
                    {
                        string argtname6 = "EN";
                        Unit argu3 = null;
                        withBlock.labEN2.Text = Expression.Term(ref argtname6, u: ref argu3);
                    }
                    else
                    {
                        string argtname7 = "EN";
                        withBlock.labEN2.Text = Expression.Term(ref argtname7, ref ru);
                    }

                    // ＥＮ数値
                    object argIndex8 = "データ不明";
                    if (ru.IsConditionSatisfied(ref argIndex8))
                    {
                        withBlock.txtEN2.Text = "???/???";
                    }
                    else
                    {
                        if (ru.EN < 1000)
                        {
                            string argbuf3 = SrcFormatter.Format(ru.EN);
                            buf = GeneralLib.LeftPaddedString(ref argbuf3, (short)GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxEN)), 3));
                        }
                        else
                        {
                            buf = "???";
                        }

                        if (ru.MaxEN < 1000)
                        {
                            buf = buf + "/" + SrcFormatter.Format(ru.MaxEN);
                        }
                        else
                        {
                            buf = buf + "/???";
                        }

                        withBlock.txtEN2.Text = buf;
                    }

                    // ＥＮゲージ
                    withBlock.picEN2.Cls();
                    if (ru.EN > 0 | i < num)
                    {
                        withBlock.picEN2.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    }

                    // 表示内容を記録
                    RightUnit = ru;
                    RightUnitHPRatio = ru.HP / (double)ru.MaxHP;
                    RightUnitENRatio = ru.EN / (double)ru.MaxEN;
                }

                // 前回の表示からのＨＰ、ＥＮの変化をアニメ表示

                // 変化がない場合はアニメ表示の必要がないのでチェックしておく
                num = 0;
                if (lu is object)
                {
                    if (lu.HP / (double)lu.MaxHP != LeftUnitHPRatio | lu.EN / (double)lu.MaxEN != LeftUnitENRatio)
                    {
                        num = 8;
                    }
                }

                if (ru is object)
                {
                    if (ru.HP != RightUnitHPRatio | ru.EN != RightUnitENRatio)
                    {
                        num = 8;
                    }
                }

                // 右ボタンが押されている場合はアニメーション表示を短縮化
                if (num > 0)
                {
                    if (IsRButtonPressed())
                    {
                        num = 2;
                    }
                }

                var loopTo = num;
                for (i = 1; i <= loopTo; i++)
                {
                    // 左側のユニット
                    if (lu is object)
                    {
                        // ＨＰ
                        if (lu.HP / (double)lu.MaxHP != LeftUnitHPRatio)
                        {
                            tmp = (int)((long)(lu.MaxHP * LeftUnitHPRatio * (num - i) + lu.HP * i) / num);
                            object argIndex9 = "データ不明";
                            if (lu.IsConditionSatisfied(ref argIndex9))
                            {
                                withBlock.txtHP1.Text = "?????/?????";
                            }
                            else
                            {
                                if (lu.HP < 100000)
                                {
                                    string argbuf4 = SrcFormatter.Format(tmp);
                                    buf = GeneralLib.LeftPaddedString(ref argbuf4, (short)GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxHP)), 5));
                                }
                                else
                                {
                                    buf = "?????";
                                }

                                if (lu.MaxHP < 100000)
                                {
                                    buf = buf + "/" + SrcFormatter.Format(lu.MaxHP);
                                }
                                else
                                {
                                    buf = buf + "/?????";
                                }

                                withBlock.txtHP1.Text = buf;
                            }

                            withBlock.picHP1.Cls();
                            if (lu.HP > 0 | i < num)
                            {
                                withBlock.picHP1.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                            }
                        }

                        // ＥＮ
                        if (lu.EN / (double)lu.MaxEN != LeftUnitENRatio)
                        {
                            tmp = (int)((long)(lu.MaxEN * LeftUnitENRatio * (num - i) + lu.EN * i) / num);
                            object argIndex10 = "データ不明";
                            if (lu.IsConditionSatisfied(ref argIndex10))
                            {
                                withBlock.txtEN1.Text = "???/???";
                            }
                            else
                            {
                                if (lu.EN < 1000)
                                {
                                    string argbuf5 = SrcFormatter.Format(tmp);
                                    buf = GeneralLib.LeftPaddedString(ref argbuf5, (short)GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxEN)), 3));
                                }
                                else
                                {
                                    buf = "???";
                                }

                                if (lu.MaxEN < 1000)
                                {
                                    buf = buf + "/" + SrcFormatter.Format(lu.MaxEN);
                                }
                                else
                                {
                                    buf = buf + "/???";
                                }

                                withBlock.txtEN1.Text = buf;
                            }

                            withBlock.picEN1.Cls();
                            if (lu.EN > 0 | i < num)
                            {
                                withBlock.picEN1.Line(-1, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                            }
                        }
                    }

                    // 右側のユニット
                    if (ru is object)
                    {
                        // ＨＰ
                        if (ru.HP / (double)ru.MaxHP != RightUnitHPRatio)
                        {
                            tmp = (int)((long)(ru.MaxHP * RightUnitHPRatio * (num - i) + ru.HP * i) / num);
                            object argIndex11 = "データ不明";
                            if (ru.IsConditionSatisfied(ref argIndex11))
                            {
                                withBlock.txtHP2.Text = "?????/?????";
                            }
                            else
                            {
                                if (ru.HP < 100000)
                                {
                                    string argbuf6 = SrcFormatter.Format(tmp);
                                    buf = GeneralLib.LeftPaddedString(ref argbuf6, (short)GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxHP)), 5));
                                }
                                else
                                {
                                    buf = "?????";
                                }

                                if (ru.MaxHP < 100000)
                                {
                                    buf = buf + "/" + SrcFormatter.Format(ru.MaxHP);
                                }
                                else
                                {
                                    buf = buf + "/?????";
                                }

                                withBlock.txtHP2.Text = buf;
                            }

                            withBlock.picHP2.Cls();
                            if (ru.HP > 0 | i < num)
                            {
                                withBlock.picHP2.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                            }
                        }

                        // ＥＮ
                        if (ru.EN / (double)ru.MaxEN != RightUnitENRatio)
                        {
                            tmp = (int)((long)(ru.MaxEN * RightUnitENRatio * (num - i) + ru.EN * i) / num);
                            object argIndex12 = "データ不明";
                            if (ru.IsConditionSatisfied(ref argIndex12))
                            {
                                withBlock.txtEN2.Text = "???/???";
                            }
                            else
                            {
                                if (ru.EN < 1000)
                                {
                                    string argbuf7 = SrcFormatter.Format(tmp);
                                    buf = GeneralLib.LeftPaddedString(ref argbuf7, (short)GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxEN)), 3));
                                }
                                else
                                {
                                    buf = "???";
                                }

                                if (ru.MaxEN < 1000)
                                {
                                    buf = buf + "/" + SrcFormatter.Format(ru.MaxEN);
                                }
                                else
                                {
                                    buf = buf + "/???";
                                }

                                withBlock.txtEN2.Text = buf;
                            }

                            withBlock.picEN2.Cls();
                            if (ru.EN > 0 | i < num)
                            {
                                withBlock.picEN2.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                            }
                        }
                    }

                    // リフレッシュ
                    if (lu is object)
                    {
                        if (lu.HP / (double)lu.MaxHP != LeftUnitHPRatio)
                        {
                            withBlock.picHP1.Refresh();
                            withBlock.txtHP1.Refresh();
                        }

                        if (lu.EN / (double)lu.MaxEN != LeftUnitENRatio)
                        {
                            withBlock.picEN1.Refresh();
                            withBlock.txtEN1.Refresh();
                        }
                    }

                    if (ru is object)
                    {
                        if (ru.HP / (double)ru.MaxHP != RightUnitHPRatio)
                        {
                            withBlock.picHP2.Refresh();
                            withBlock.txtHP2.Refresh();
                        }

                        if (ru.EN / (double)ru.MaxEN != RightUnitENRatio)
                        {
                            withBlock.picEN2.Refresh();
                            withBlock.txtEN2.Refresh();
                        }
                    }

                    Sleep(20);
                }

                // 表示内容を記録
                if (lu is object)
                {
                    LeftUnitHPRatio = lu.HP / (double)lu.MaxHP;
                    LeftUnitENRatio = lu.EN / (double)lu.MaxEN;
                }

                if (ru is object)
                {
                    RightUnitHPRatio = ru.HP / (double)ru.MaxHP;
                    RightUnitENRatio = ru.EN / (double)ru.MaxEN;
                }

                Application.DoEvents();
            }
        }

        // メッセージウィンドウの状態を記録する
        public static void SaveMessageFormStatus()
        {
            IsMessageFormVisible = My.MyProject.Forms.frmMessage.Visible;
            SavedLeftUnit = LeftUnit;
            SavedRightUnit = RightUnit;
        }

        // メッセージウィンドウの状態を記録した状態に保つ
        public static void KeepMessageFormStatus()
        {
            if (!IsMessageFormVisible)
            {
                // 記録した時点でメッセージウィンドウが表示されていなければ
                if (My.MyProject.Forms.frmMessage.Visible)
                {
                    // 開いているメッセージウィンドウを強制的に閉じる
                    CloseMessageForm();
                }
            }
            else if (!My.MyProject.Forms.frmMessage.Visible)
            {
                // 記録した時点ではメッセージウィンドウが表示されていたので、
                // メッセージウィンドウが表示されていない場合は表示する
                OpenMessageForm(ref SavedLeftUnit, ref SavedRightUnit);
            }
            else if (LeftUnit is null & RightUnit is null & (SavedLeftUnit is object | SavedRightUnit is object))
            {
                // メッセージウィンドウからユニット表示が消えてしまった場合は再表示
                OpenMessageForm(ref SavedLeftUnit, ref SavedRightUnit);
            }
        }


        // === メッセージ表示に関する処理 ===

        // メッセージウィンドウにメッセージを表示
        public static void DisplayMessage(ref string pname, string msg, string msg_mode = "")
        {
            string[] messages;
            short msg_head, line_head;
            short i, j;
            string buf, ch;
            PictureBox p;
            string pnickname = default, fname;
            int start_time, wait_time;
            short lnum, prev_lnum;
            var PT = default(POINTAPI);
            bool is_automode;
            short lstate, rstate;
            bool in_tag;
            var is_character_message = default(bool);
            var cl_margin = new float[3];
            var left_margin = default(string);

            // キャラ表示の描き換え
            if (pname == "システム")
            {
                // 「システム」
                My.MyProject.Forms.frmMessage.picFace.Image = Image.FromFile("");
                My.MyProject.Forms.frmMessage.picFace.Refresh();
                DisplayedPilot = "";
                left_margin = "";
            }
            else if (!string.IsNullOrEmpty(pname))
            {
                // どのキャラ画像を使うか？
                bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                bool localIsDefined1() { object argIndex1 = pname; var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                object argIndex1 = pname;
                if (SRC.PList.IsDefined(ref argIndex1))
                {
                    Pilot localItem() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                    pnickname = localItem().get_Nickname(false);
                    Pilot localItem1() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                    fname = localItem1().get_Bitmap(false);
                }
                else if (localIsDefined())
                {
                    PilotData localItem2() { object argIndex1 = pname; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                    pnickname = localItem2().Nickname;
                    PilotData localItem3() { object argIndex1 = pname; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                    fname = localItem3().Bitmap;
                }
                else if (localIsDefined1())
                {
                    NonPilotData localItem4() { object argIndex1 = pname; var ret = SRC.NPDList.Item(ref argIndex1); return ret; }

                    pnickname = localItem4().Nickname;
                    NonPilotData localItem5() { object argIndex1 = pname; var ret = SRC.NPDList.Item(ref argIndex1); return ret; }

                    fname = localItem5().Bitmap;
                }
                else
                {
                    fname = "-.bmp";
                }

                // キャラ画像の表示
                if (fname != "-.bmp")
                {
                    fname = @"Pilot\" + fname;
                    if ((DisplayedPilot ?? "") != (fname ?? "") | (DisplayMode ?? "") != (msg_mode ?? ""))
                    {
                        string argdraw_option = "メッセージ " + msg_mode;
                        if (DrawPicture(ref fname, 0, 0, 64, 64, 0, 0, 0, 0, ref argdraw_option))
                        {
                            My.MyProject.Forms.frmMessage.picFace.Refresh();
                            DisplayedPilot = fname;
                            DisplayMode = msg_mode;
                        }
                        else
                        {
                            My.MyProject.Forms.frmMessage.picFace.Image = Image.FromFile("");
                            My.MyProject.Forms.frmMessage.picFace.Refresh();
                            DisplayedPilot = "";
                            DisplayMode = "";

                            // パイロット画像が存在しないことを記録しておく
                            bool localIsDefined2() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                            bool localIsDefined3() { object argIndex1 = pname; var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                            object argIndex3 = pname;
                            if (SRC.PList.IsDefined(ref argIndex3))
                            {
                                object argIndex2 = pname;
                                {
                                    var withBlock = SRC.PList.Item(ref argIndex2);
                                    if ((withBlock.get_Bitmap(false) ?? "") == (withBlock.Data.Bitmap ?? ""))
                                    {
                                        withBlock.Data.IsBitmapMissing = true;
                                    }
                                }
                            }
                            else if (localIsDefined2())
                            {
                                PilotData localItem6() { object argIndex1 = pname; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                                localItem6().IsBitmapMissing = true;
                            }
                            else if (localIsDefined3())
                            {
                                NonPilotData localItem7() { object argIndex1 = pname; var ret = SRC.NPDList.Item(ref argIndex1); return ret; }

                                localItem7().IsBitmapMissing = true;
                            }
                        }
                    }
                }
                else
                {
                    My.MyProject.Forms.frmMessage.picFace.Image = Image.FromFile("");
                    My.MyProject.Forms.frmMessage.picFace.Refresh();
                    DisplayedPilot = "";
                    DisplayMode = "";
                }

                string argoname = "会話パイロット名改行";
                if (Expression.IsOptionDefined(ref argoname))
                {
                    left_margin = " ";
                }
                else
                {
                    left_margin = "  ";
                }
            }

            // メッセージ中の式置換を処理
            Expression.FormatMessage(ref msg);
            msg = Strings.Trim(msg);

            // 末尾に強制改行が入っている場合は取り除く
            while (Strings.Right(msg, 1) == ";")
                msg = Strings.Left(msg, Strings.Len(msg) - 1);

            // メッセージが空の場合はキャラ表示の描き換えのみ行う
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            switch (pname ?? "")
            {
                // そのまま使用
                case "システム":
                    {
                        break;
                    }

                case var @case when @case == "":
                    {
                        // 基本的にはそのまま使用するが、せりふ表示の代用の場合は
                        // せりふ表示用の処理を行う
                        i = 0;
                        if (Strings.InStr(msg, "「") > 0 & Strings.Right(msg, 1) == "」")
                        {
                            i = (short)Strings.InStr(msg, "「");
                        }
                        else if (Strings.InStr(msg, "『") > 0 & Strings.Right(msg, 1) == "』")
                        {
                            i = (short)Strings.InStr(msg, "『");
                        }
                        else if (Strings.InStr(msg, "(") > 0 & Strings.Right(msg, 1) == ")")
                        {
                            i = (short)Strings.InStr(msg, "(");
                        }
                        else if (Strings.InStr(msg, "（") > 0 & Strings.Right(msg, 1) == "）")
                        {
                            i = (short)Strings.InStr(msg, "（");
                        }

                        if (i > 1)
                        {
                            bool localIsDefined4() { object argIndex1 = Strings.Trim(Strings.Left(msg, i - 1)); var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                            bool localIsDefined5() { object argIndex1 = Strings.Trim(Strings.Left(msg, i - 1)); var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                            if (i < 8 | localIsDefined4() | localIsDefined5())
                            {
                                is_character_message = true;
                                bool localIsSpace() { string argch = Strings.Mid(msg, i - 1, 1); var ret = GeneralLib.IsSpace(ref argch); return ret; }

                                if (!localIsSpace())
                                {
                                    // "「"の前に半角スペースを挿入
                                    msg = Strings.Left(msg, i - 1) + " " + Strings.Mid(msg, i);
                                }
                            }
                        }

                        break;
                    }

                default:
                    {
                        is_character_message = true;
                        if ((Strings.Left(msg, 1) == "(" | Strings.Left(msg, 1) == "（") & (Strings.Right(msg, 1) == ")" | Strings.Right(msg, 1) == "）"))
                        {
                            // モノローグ
                            msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                            string argoname1 = "会話パイロット名改行";
                            msg = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(pnickname, Interaction.IIf(Expression.IsOptionDefined(ref argoname1), ";", " ")), "（"), msg), "）"));
                        }
                        else if (Strings.Left(msg, 1) == "『" & Strings.Right(msg, 1) == "』")
                        {
                            msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                            string argoname3 = "会話パイロット名改行";
                            msg = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(pnickname, Interaction.IIf(Expression.IsOptionDefined(ref argoname3), ";", " ")), "『"), msg), "』"));
                        }
                        else
                        {
                            // せりふ
                            string argoname2 = "会話パイロット名改行";
                            msg = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(pnickname, Interaction.IIf(Expression.IsOptionDefined(ref argoname2), ";", " ")), "「"), msg), "」"));
                        }

                        break;
                    }
            }

            // 強制改行の位置を設定
            string argoname4 = "改行時余白短縮";
            if (Expression.IsOptionDefined(ref argoname4))
            {
                cl_margin[0] = 0.94f; // メッセージ長の超過による改行の位置
                cl_margin[1] = 0.7f; // "。"," "による改行の位置
                cl_margin[2] = 0.85f; // "、"による改行の位置
            }
            else
            {
                cl_margin[0] = 0.8f;
                cl_margin[1] = 0.6f;
                cl_margin[2] = 0.75f;
            }

            // メッセージを分割
            messages = new string[2];
            msg_head = 1;
            buf = "";
            var loopTo = (short)Strings.Len(msg);
            for (i = 1; i <= loopTo; i++)
            {
                if (Strings.Mid(msg, i, 1) == ":")
                {
                    buf = buf + Strings.Mid(msg, msg_head, i - msg_head);
                    messages[Information.UBound(messages)] = buf;
                    Array.Resize(ref messages, Information.UBound(messages) + 1 + 1);
                    msg_head = (short)(i + 1);
                }
            }

            messages[Information.UBound(messages)] = buf + Strings.Mid(msg, msg_head);

            // メッセージ長判定のため、元のメッセージを再構築
            msg = "";
            var loopTo1 = (short)Information.UBound(messages);
            for (i = 1; i <= loopTo1; i++)
                msg = msg + messages[i];

            // メッセージの表示
            p = My.MyProject.Forms.frmMessage.picMessage;
            msg_head = 1;
            prev_lnum = 0;
            i = 0;
            short counter;
            while (i < Information.UBound(messages))
            {
                i = (short)(i + 1);
                buf = messages[i];
                lnum = 0;
                line_head = msg_head;
                in_tag = false;

                p.Cls();
                p.CurrentX = 1;
                if (msg_head == 1)
                {
                    // フォント設定を初期化
                    p.Font = SrcFormatter.FontChangeBold(p.Font, false);
                    p.Font = SrcFormatter.FontChangeItalic(p.Font, false);
                    p.Font = SrcFormatter.FontChangeName(p.Font, "ＭＳ Ｐ明朝");
                    p.Font = SrcFormatter.FontChangeSize(p.Font, 12f);
                    p.ForeColor = Color.Black;
                }
                // メッセージの途中から表示
                else if (is_character_message)
                {
                    p.Print("  ");
                }

                counter = msg_head;
                var loopTo2 = (short)Strings.Len(buf);
                for (j = counter; j <= loopTo2; j++)
                {
                    ch = Strings.Mid(buf, j, 1);

                    // ";"では必ず改行
                    if (ch == ";")
                    {
                        if (j != line_head)
                        {
                            string argmsg = Strings.Mid(buf, line_head, j - line_head);
                            PrintMessage(ref argmsg);
                            lnum = (short)(lnum + 1);
                            string argoname5 = "会話パイロット名改行";
                            string argoname6 = "会話パイロット名改行";
                            if (is_character_message & (lnum > 1 & Expression.IsOptionDefined(ref argoname5) | lnum > 0 & !Expression.IsOptionDefined(ref argoname6)))
                            {
                                p.Print(left_margin);
                            }
                        }

                        line_head = (short)(j + 1);
                        goto NextLoop;
                    }

                    // タグ内では改行しない
                    if (ch == "<")
                    {
                        in_tag = true;
                        goto NextLoop;
                    }
                    else if (ch == ">")
                    {
                        in_tag = false;
                    }
                    else if (in_tag)
                    {
                        goto NextLoop;
                    }

                    // メッセージが途切れてしまう場合は必ず改行
                    if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > 0.95d * SrcFormatter.PixelsToTwipsX(p.Width))
                    {
                        string argmsg1 = Strings.Mid(buf, line_head, j - line_head + 1);
                        PrintMessage(ref argmsg1);
                        lnum = (short)(lnum + 1);
                        string argoname7 = "会話パイロット名改行";
                        string argoname8 = "会話パイロット名改行";
                        if (is_character_message & (lnum > 1 & Expression.IsOptionDefined(ref argoname7) | lnum > 0 & !Expression.IsOptionDefined(ref argoname8)))
                        {
                            p.Print(left_margin);
                        }

                        line_head = (short)(j + 1);
                        goto NextLoop;
                    }

                    // 禁則処理
                    switch (Strings.Mid(buf, j + 1, 1) ?? "")
                    {
                        case "。":
                        case "、":
                        case "…":
                        case "‥":
                        case "・":
                        case "･":
                        case "～":
                        case "ー":
                        case "－":
                        case "！":
                        case "？":
                        case "」":
                        case "』":
                        case "）":
                        case ")":
                        case " ":
                        case ";":
                            {
                                goto NextLoop;
                                break;
                            }
                    }

                    switch (Strings.Mid(buf, j + 2, 1) ?? "")
                    {
                        case "。":
                        case "、":
                        case "…":
                        case "‥":
                        case "・":
                        case "･":
                        case "～":
                        case "ー":
                        case "－":
                        case "！":
                        case "？":
                        case "」":
                        case "』":
                        case "）":
                        case ")":
                        case " ":
                        case ";":
                            {
                                goto NextLoop;
                                break;
                            }
                    }

                    if (Strings.Mid(buf, j + 3, 1) == ";")
                    {
                        goto NextLoop;
                    }

                    // 改行の判定
                    if (MessageLen(Strings.Mid(messages[i], line_head)) < 0.95d * SrcFormatter.PixelsToTwipsX(p.Width))
                    {
                        // 全体が一行に収まる場合
                        goto NextLoop;
                    }

                    switch (ch ?? "")
                    {
                        case "。":
                            {
                                if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[1] * SrcFormatter.PixelsToTwipsX(p.Width))
                                {
                                    string argmsg2 = Strings.Mid(buf, line_head, j - line_head + 1);
                                    PrintMessage(ref argmsg2);
                                    lnum = (short)(lnum + 1);
                                    string argoname9 = "会話パイロット名改行";
                                    string argoname10 = "会話パイロット名改行";
                                    if (is_character_message & (lnum > 1 & Expression.IsOptionDefined(ref argoname9) | lnum > 0 & !Expression.IsOptionDefined(ref argoname10)))
                                    {
                                        p.Print(left_margin);
                                    }

                                    line_head = (short)(j + 1);
                                }

                                break;
                            }

                        case "、":
                            {
                                if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[2] * SrcFormatter.PixelsToTwipsX(p.Width))
                                {
                                    string argmsg3 = Strings.Mid(buf, line_head, j - line_head + 1);
                                    PrintMessage(ref argmsg3);
                                    lnum = (short)(lnum + 1);
                                    string argoname11 = "会話パイロット名改行";
                                    string argoname12 = "会話パイロット名改行";
                                    if (is_character_message & (lnum > 1 & Expression.IsOptionDefined(ref argoname11) | lnum > 0 & !Expression.IsOptionDefined(ref argoname12)))
                                    {
                                        p.Print(left_margin);
                                    }

                                    line_head = (short)(j + 1);
                                }

                                break;
                            }

                        case " ":
                            {
                                ch = Strings.Mid(buf, j - 1, 1);
                                // スペースが文の区切りに使われているかどうか判定
                                if (pname != "システム" & (ch == "！" | ch == "？" | ch == "…" | ch == "‥" | ch == "・" | ch == "･" | ch == "～"))
                                {
                                    // 文の区切り
                                    if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[1] * SrcFormatter.PixelsToTwipsX(p.Width))
                                    {
                                        string argmsg4 = Strings.Mid(buf, line_head, j - line_head + 1);
                                        PrintMessage(ref argmsg4);
                                        lnum = (short)(lnum + 1);
                                        string argoname13 = "会話パイロット名改行";
                                        string argoname14 = "会話パイロット名改行";
                                        if (is_character_message & (lnum > 1 & Expression.IsOptionDefined(ref argoname13) | lnum > 0 & !Expression.IsOptionDefined(ref argoname14)))
                                        {
                                            p.Print(left_margin);
                                        }

                                        line_head = (short)(j + 1);
                                    }
                                }
                                // 単なる空白
                                else if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[0] * SrcFormatter.PixelsToTwipsX(p.Width))
                                {
                                    string argmsg5 = Strings.Mid(buf, line_head, j - line_head + 1);
                                    PrintMessage(ref argmsg5);
                                    lnum = (short)(lnum + 1);
                                    string argoname15 = "会話パイロット名改行";
                                    string argoname16 = "会話パイロット名改行";
                                    if (is_character_message & (lnum > 1 & Expression.IsOptionDefined(ref argoname15) | lnum > 0 & !Expression.IsOptionDefined(ref argoname16)))
                                    {
                                        p.Print(left_margin);
                                    }

                                    line_head = (short)(j + 1);
                                }

                                break;
                            }

                        default:
                            {
                                if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[0] * SrcFormatter.PixelsToTwipsX(p.Width))
                                {
                                    string argmsg6 = Strings.Mid(buf, line_head, j - line_head + 1);
                                    PrintMessage(ref argmsg6);
                                    lnum = (short)(lnum + 1);
                                    string argoname17 = "会話パイロット名改行";
                                    string argoname18 = "会話パイロット名改行";
                                    if (is_character_message & (lnum > 1 & Expression.IsOptionDefined(ref argoname17) | lnum > 0 & !Expression.IsOptionDefined(ref argoname18)))
                                    {
                                        p.Print(left_margin);
                                    }

                                    line_head = (short)(j + 1);
                                }

                                break;
                            }
                    }

                    NextLoop:
                    ;
                    if (lnum == 4)
                    {
                        if (j < Strings.Len(buf))
                        {
                            msg_head = line_head;
                            i = (short)(i - 1);
                            break;
                        }
                    }
                }
                // 残りの部分を表示
                if (lnum < 4)
                {
                    if (Strings.Len(buf) >= line_head)
                    {
                        string argmsg7 = Strings.Mid(buf, line_head);
                        PrintMessage(ref argmsg7);
                    }
                }

                Application.DoEvents();
                if (MessageWait > 10000)
                {
                    AutoMessageMode = false;
                }

                // ウィンドウのキャプションを設定
                if (AutoMessageMode)
                {
                    if (My.MyProject.Forms.frmMessage.Text == "メッセージ")
                    {
                        My.MyProject.Forms.frmMessage.Text = "メッセージ (自動送り)";
                    }
                }
                else if (My.MyProject.Forms.frmMessage.Text == "メッセージ (自動送り)")
                {
                    My.MyProject.Forms.frmMessage.Text = "メッセージ";
                }

                // 次のメッセージ表示までの時間を設定(自動メッセージ送り用)
                start_time = GeneralLib.timeGetTime();
                wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250);

                // 次のメッセージ待ち
                IsFormClicked = false;
                is_automode = AutoMessageMode;
                while (!IsFormClicked)
                {
                    if (AutoMessageMode)
                    {
                        if (start_time + wait_time < GeneralLib.timeGetTime())
                        {
                            break;
                        }
                    }

                    GetCursorPos(ref PT);

                    // メッセージウインドウ上でマウスボタンを押した場合
                    if (ReferenceEquals(Form.ActiveForm, My.MyProject.Forms.m_frmMessage))
                    {
                        {
                            var withBlock1 = My.MyProject.Forms.frmMessage;
                            if ((long)SrcFormatter.PixelsToTwipsX(withBlock1.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X & PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX() & (long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y & PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock1.Top) + SrcFormatter.PixelsToTwipsY(withBlock1.Height)) / (long)SrcFormatter.TwipsPerPixelY())
                            {
                                lstate = GetAsyncKeyState(LButtonID);
                                rstate = GetAsyncKeyState(RButtonID);
                                if ((lstate & 0x8000) != 0)
                                {
                                    if (start_time + wait_time < GeneralLib.timeGetTime())
                                    {
                                        // 左ボタンでメッセージの自動送り
                                        break;
                                    }
                                }
                                else if ((rstate & 0x8000) != 0)
                                {
                                    // 右ボタンでメッセージの早送り
                                    break;
                                }
                            }
                        }
                    }

                    // メインウインドウ上でマウスボタンを押した場合
                    if (ReferenceEquals(Form.ActiveForm, MainForm))
                    {
                        {
                            var withBlock2 = MainForm;
                            if ((long)SrcFormatter.PixelsToTwipsX(withBlock2.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X & PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock2.Left) + SrcFormatter.PixelsToTwipsX(withBlock2.Width)) / (long)SrcFormatter.TwipsPerPixelX() & (long)SrcFormatter.PixelsToTwipsY(withBlock2.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y & PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock2.Top) + SrcFormatter.PixelsToTwipsY(withBlock2.Height)) / (long)SrcFormatter.TwipsPerPixelY())
                            {
                                lstate = GetAsyncKeyState(LButtonID);
                                rstate = GetAsyncKeyState(RButtonID);
                                if ((lstate & 0x8000) != 0)
                                {
                                    if (start_time + wait_time < GeneralLib.timeGetTime())
                                    {
                                        // 左ボタンでメッセージの自動送り
                                        break;
                                    }
                                }
                                else if ((rstate & 0x8000) != 0)
                                {
                                    // 右ボタンでメッセージの早送り
                                    break;
                                }
                            }
                        }
                    }

                    Sleep(100);
                    Application.DoEvents();

                    // 自動送りモードが切り替えられた場合
                    if (is_automode != AutoMessageMode)
                    {
                        IsFormClicked = false;
                        is_automode = AutoMessageMode;
                        if (AutoMessageMode)
                        {
                            My.MyProject.Forms.frmMessage.Text = "メッセージ (自動送り)";
                            start_time = GeneralLib.timeGetTime();
                            wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250);
                        }
                        else
                        {
                            My.MyProject.Forms.frmMessage.Text = "メッセージ";
                        }
                    }
                }

                // ウェイト計算用に既に表示した行数を記録
                if (lnum < 4)
                {
                    prev_lnum = lnum;
                }
                else
                {
                    prev_lnum = 0;
                }
            }

            // フォント設定を元に戻す
            p.Font = SrcFormatter.FontChangeBold(p.Font, false);
            p.Font = SrcFormatter.FontChangeItalic(p.Font, false);
            p.Font = SrcFormatter.FontChangeName(p.Font, "ＭＳ Ｐ明朝");
            p.Font = SrcFormatter.FontChangeSize(p.Font, 12f);
            p.ForeColor = Color.Black;
            return;
            ErrorHandler:
            ;
            string argmsg8 = "パイロット用画像ファイル" + Constants.vbCr + Constants.vbLf + DisplayedPilot + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。";
            ErrorMessage(ref argmsg8);
        }

        // メッセージウィンドウに文字列を書き込む
        public static void PrintMessage(ref string msg, bool is_sys_msg = false)
        {
            string buf, tag, ch;
            PictureBox p;
            short last_y, last_x, max_y = default;
            short i, head;
            var in_tag = default(bool);
            var escape_depth = default(short);
            p = My.MyProject.Forms.frmMessage.picMessage;
            head = 1;
            string cname;
            var loopTo = (short)Strings.Len(msg);
            for (i = 1; i <= loopTo; i++)
            {
                ch = Strings.Mid(msg, i, 1);

                // システムメッセージの時のみエスケープシーケンスの処理を行う
                if (is_sys_msg)
                {
                    switch (ch ?? "")
                    {
                        case "[":
                            {
                                escape_depth = (short)(escape_depth + 1);
                                if (escape_depth == 1)
                                {
                                    // エスケープシーケンス開始
                                    // それまでの文字列を出力
                                    p.Print(Strings.Mid(msg, head, i - head));
                                    head = (short)(i + 1);
                                    goto NextChar;
                                }

                                break;
                            }

                        case "]":
                            {
                                escape_depth = (short)(escape_depth - 1);
                                if (escape_depth == 0)
                                {
                                    // エスケープシーケンス終了
                                    // エスケープシーケンスを出力
                                    p.Print(Strings.Mid(msg, head, i - head));
                                    head = (short)(i + 1);
                                    goto NextChar;
                                }

                                break;
                            }
                    }
                }

                // タグの処理
                switch (ch ?? "")
                {
                    case "<":
                        {
                            if (!in_tag & escape_depth == 0)
                            {
                                // タグ開始
                                in_tag = true;
                                // それまでの文字列を出力
                                p.Print(Strings.Mid(msg, head, i - head));
                                head = (short)(i + 1);
                                goto NextChar;
                            }

                            break;
                        }

                    case ">":
                        {
                            if (in_tag)
                            {
                                // タグ終了
                                in_tag = false;

                                // タグの切り出し
                                tag = Strings.LCase(Strings.Mid(msg, head, i - head));

                                // タグに合わせて各種処理を行う
                                switch (tag ?? "")
                                {
                                    case "b":
                                        {
                                            p.Font = SrcFormatter.FontChangeBold(p.Font, true);
                                            break;
                                        }

                                    case "/b":
                                        {
                                            p.Font = SrcFormatter.FontChangeBold(p.Font, false);
                                            break;
                                        }

                                    case "i":
                                        {
                                            p.Font = SrcFormatter.FontChangeItalic(p.Font, true);
                                            break;
                                        }

                                    case "/i":
                                        {
                                            p.Font = SrcFormatter.FontChangeItalic(p.Font, false);
                                            break;
                                        }

                                    case "big":
                                        {
                                            p.Font = SrcFormatter.FontChangeSize(p.Font, p.Font.SizeInPoints + 2f);
                                            last_x = p.CurrentX;
                                            last_y = p.CurrentY;
                                            p.Print();
                                            if (p.CurrentY > max_y)
                                            {
                                                max_y = p.CurrentY;
                                            }
                                            p.CurrentX = last_x;
                                            p.CurrentY = last_y;
                                            break;
                                        }

                                    case "/big":
                                        {
                                            p.Font = SrcFormatter.FontChangeSize(p.Font, p.Font.SizeInPoints - 2f);
                                            break;
                                        }

                                    case "small":
                                        {
                                            p.Font = SrcFormatter.FontChangeSize(p.Font, p.Font.SizeInPoints - 2f);
                                            last_x = p.CurrentX;
                                            last_y = p.CurrentY;
                                            p.Print();
                                            if (p.CurrentY > max_y)
                                            {
                                                max_y = p.CurrentY;
                                            }
                                            p.CurrentX = last_x;
                                            p.CurrentY = last_y;
                                            break;
                                        }

                                    case "/small":
                                        {
                                            p.Font = SrcFormatter.FontChangeSize(p.Font, p.Font.SizeInPoints + 2f);
                                            break;
                                        }

                                    case "/color":
                                        {
                                            p.ForeColor = Color.Black;
                                            break;
                                        }

                                    case "/size":
                                        {
                                            p.Font = SrcFormatter.FontChangeSize(p.Font, 12f);
                                            break;
                                        }

                                    case "lt":
                                        {
                                            p.Print("<");
                                            break;
                                        }

                                    case "gt":
                                        {
                                            p.Print(">");
                                            break;
                                        }

                                    default:
                                        {
                                            if (Strings.InStr(tag, "color=") == 1)
                                            {
                                                // 色設定
                                                string argexpr = Strings.Mid(tag, 7);
                                                cname = Expression.GetValueAsString(ref argexpr);
                                                switch (cname ?? "")
                                                {
                                                    case "black":
                                                        {
                                                            p.ForeColor = Color.Black;
                                                            break;
                                                        }

                                                    case "gray":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x80, 0x80, 0x80));
                                                            break;
                                                        }

                                                    case "silver":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0xC0, 0xC0, 0xC0));
                                                            break;
                                                        }

                                                    case "white":
                                                        {
                                                            p.ForeColor = Color.White;
                                                            break;
                                                        }

                                                    case "red":
                                                        {
                                                            p.ForeColor = Color.Red;
                                                            break;
                                                        }

                                                    case "yellow":
                                                        {
                                                            p.ForeColor = Color.Yellow;
                                                            break;
                                                        }

                                                    case "lime":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x0, 0xFF, 0x0));
                                                            break;
                                                        }

                                                    case "aqua":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x0, 0xFF, 0xFF));
                                                            break;
                                                        }

                                                    case "blue":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x0, 0x0, 0xFF));
                                                            break;
                                                        }

                                                    case "fuchsia":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0xFF, 0x0, 0xFF));
                                                            break;
                                                        }

                                                    case "maroon":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x80, 0x0, 0x0));
                                                            break;
                                                        }

                                                    case "olive":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x80, 0x80, 0x0));
                                                            break;
                                                        }

                                                    case "green":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x0, 0x80, 0x0));
                                                            break;
                                                        }

                                                    case "teal":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x0, 0x80, 0x80));
                                                            break;
                                                        }

                                                    case "navy":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x0, 0x0, 0x80));
                                                            break;
                                                        }

                                                    case "purple":
                                                        {
                                                            p.ForeColor = ColorTranslator.FromOle(Information.RGB(0x80, 0x0, 0x80));
                                                            break;
                                                        }

                                                    default:
                                                        {
                                                            if (Strings.Asc(cname) == 35) // #
                                                            {
                                                                buf = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                                                                StringType.MidStmtStr(ref buf, 1, 2, "&H");
                                                                var midTmp = Strings.Mid(cname, 6, 2);
                                                                StringType.MidStmtStr(ref buf, 3, 2, midTmp);
                                                                var midTmp1 = Strings.Mid(cname, 4, 2);
                                                                StringType.MidStmtStr(ref buf, 5, 2, midTmp1);
                                                                var midTmp2 = Strings.Mid(cname, 2, 2);
                                                                StringType.MidStmtStr(ref buf, 7, 2, midTmp2);
                                                                if (Information.IsNumeric(buf))
                                                                {
                                                                    p.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(buf));
                                                                }
                                                            }

                                                            break;
                                                        }
                                                }
                                            }
                                            else if (Strings.InStr(tag, "size=") == 1)
                                            {
                                                // サイズ設定
                                                if (Information.IsNumeric(Strings.Mid(tag, 6)))
                                                {
                                                    p.Font = SrcFormatter.FontChangeSize(p.Font, Conversions.ToInteger(Strings.Mid(tag, 6)));
                                                    last_x = p.CurrentX;
                                                    last_y = p.CurrentY;
                                                    p.Print();
                                                    if (p.CurrentY > max_y)
                                                    {
                                                        max_y = p.CurrentY;
                                                    }
                                                    p.CurrentX = last_x;
                                                    p.CurrentY = last_y;
                                                }
                                            }
                                            else
                                            {
                                                // タグではないのでそのまま書き出す
                                                p.Print(Strings.Mid(msg, head - 1, i - head + 2));
                                            }

                                            break;
                                        }
                                }

                                head = (short)(i + 1);
                                goto NextChar;
                            }

                            break;
                        }
                }

                NextChar:
                ;
            }

            // 終了していないタグ、もしくはエスケープシーケンスはただの文字列と見なす
            if (in_tag | escape_depth > 0)
            {
                head = (short)(head - 1);
            }

            // 未出力の文字列を出力する
            if (head <= Strings.Len(msg))
            {
                if (Strings.Right(msg, 1) == "」")
                {
                    // 最後の括弧の位置は一番大きなサイズの文字に合わせる
                    p.Print(Strings.Mid(msg, head, Strings.Len(msg) - head));

                    last_x = p.CurrentX;
                    last_y = p.CurrentY;
                    p.Print();
                    p.CurrentX = last_x;
                    if (p.CurrentY > max_y)
                    {
                        p.CurrentY = last_y;
                    }
                    else
                    {
                        p.CurrentY = (short)(last_y + max_y) - p.CurrentY;
                    }

                    p.Print(Strings.Right(msg, 1));
                }
                else
                {
                    p.Print(Strings.Mid(msg, head));
                }
            }
            else
            {
                // 未出力の文字列がない場合は改行のみ
                p.Print();
            }

            // 改行後の位置は一番大きなサイズの文字に合わせる
            if (max_y > p.CurrentY)
            {
                p.CurrentY = max_y + 1;
            }
            else
            {
                p.CurrentY = p.CurrentY + 1;
            }
            p.CurrentX = 1;
        }

        // メッセージ幅を計算(タグを無視して)
        public static short MessageLen(string msg)
        {
            short MessageLenRet = default;
            var buf = default(string);
            short ret;

            // タグが存在する？
            ret = (short)Strings.InStr(msg, "<");
            if (ret == 0)
            {
                MessageLenRet = My.MyProject.Forms.frmMessage.picMessage.TextWidth(msg);
                return MessageLenRet;
            }

            // タグを除いたメッセージを作成
            while (ret > 0)
            {
                buf = buf + Strings.Left(msg, ret - 1);
                msg = Strings.Mid(msg, ret + 1);
                ret = (short)Strings.InStr(msg, ">");
                if (ret > 0)
                {
                    msg = Strings.Mid(msg, ret + 1);
                }
                else
                {
                    msg = "";
                }

                ret = (short)Strings.InStr(msg, "<");
            }

            buf = buf + msg;

            // タグ抜きメッセージのピクセル幅を計算
            MessageLenRet = My.MyProject.Forms.frmMessage.picMessage.TextWidth(buf);
            return MessageLenRet;
        }

        // メッセージウィンドウに戦闘メッセージを表示
        public static void DisplayBattleMessage(ref string pname, string msg, [Optional, DefaultParameterValue("")] ref string msg_mode)
        {
            string[] messages;
            short i, j;
            short lnum = default, line_head, prev_lnum = default;
            PictureBox p;
            string buf2, buf, ch;
            var pnickname = default(string);
            int wait_time, start_time = default, cur_time;
            var need_refresh = default(bool);
            var in_tag = default(bool);
            string dx, dw, dh, dy;
            string options;
            short n, opt_n;
            string fname;
            string cname;
            var clear_every_time = default(bool);
            bool is_char_message;
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
                    Static init_display_battle_message As Boolean

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
                    Static extdata_bitmap_dir_exists As Boolean

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
                    Static extdata2_bitmap_dir_exists As Boolean

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
                    Static last_path As String

             */
            var cl_margin = new float[3];

            // 初めて実行する際に、各フォルダにBitmapフォルダがあるかチェック
            if (!init_display_battle_message)
            {
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + "Bitmap", FileAttribute.Directory)) > 0)
                {
                    extdata_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + "Bitmap", FileAttribute.Directory)) > 0)
                {
                    extdata2_bitmap_dir_exists = true;
                }

                init_display_battle_message = true;
            }

            // メッセージウィンドウが閉じられていれば表示しない
            if ((int)My.MyProject.Forms.frmMessage.WindowState == 1)
            {
                return;
            }

            // ウィンドウのキャプションを設定
            if (My.MyProject.Forms.frmMessage.Text == "メッセージ (自動送り)")
            {
                My.MyProject.Forms.frmMessage.Text = "メッセージ";
            }

            // キャラ表示の描き換え
            if (pname == "システム")
            {
                // 「システム」
                My.MyProject.Forms.frmMessage.picFace.Image = Image.FromFile("");
                My.MyProject.Forms.frmMessage.picFace.Refresh();
                DisplayedPilot = "";
            }
            else if (!string.IsNullOrEmpty(pname) & pname != "-")
            {
                // どのキャラ画像を使うか？
                bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                bool localIsDefined1() { object argIndex1 = pname; var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                object argIndex1 = pname;
                if (SRC.PList.IsDefined(ref argIndex1))
                {
                    Pilot localItem() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                    pnickname = localItem().get_Nickname(false);
                    Pilot localItem1() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                    fname = localItem1().get_Bitmap(false);
                }
                else if (localIsDefined())
                {
                    PilotData localItem2() { object argIndex1 = pname; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                    pnickname = localItem2().Nickname;
                    PilotData localItem3() { object argIndex1 = pname; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                    fname = localItem3().Bitmap;
                }
                else if (localIsDefined1())
                {
                    NonPilotData localItem4() { object argIndex1 = pname; var ret = SRC.NPDList.Item(ref argIndex1); return ret; }

                    pnickname = localItem4().Nickname;
                    NonPilotData localItem5() { object argIndex1 = pname; var ret = SRC.NPDList.Item(ref argIndex1); return ret; }

                    fname = localItem5().Bitmap;
                }
                else
                {
                    fname = "-.bmp";
                }

                // キャラ画像の表示
                if (fname != "-.bmp")
                {
                    fname = @"Pilot\" + fname;
                    if ((DisplayedPilot ?? "") != (fname ?? "") | (DisplayMode ?? "") != (msg_mode ?? ""))
                    {
                        string argdraw_option = "メッセージ " + msg_mode;
                        if (DrawPicture(ref fname, 0, 0, 64, 64, 0, 0, 0, 0, ref argdraw_option))
                        {
                            My.MyProject.Forms.frmMessage.picFace.Refresh();
                            DisplayedPilot = fname;
                            DisplayMode = msg_mode;
                        }
                        else
                        {
                            My.MyProject.Forms.frmMessage.picFace.Image = Image.FromFile("");
                            My.MyProject.Forms.frmMessage.picFace.Refresh();
                            DisplayedPilot = "";
                            DisplayMode = "";

                            // パイロット画像が存在しないことを記録しておく
                            bool localIsDefined2() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                            bool localIsDefined3() { object argIndex1 = pname; var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                            object argIndex3 = pname;
                            if (SRC.PList.IsDefined(ref argIndex3))
                            {
                                object argIndex2 = pname;
                                {
                                    var withBlock = SRC.PList.Item(ref argIndex2);
                                    if ((withBlock.get_Bitmap(false) ?? "") == (withBlock.Data.Bitmap ?? ""))
                                    {
                                        withBlock.Data.IsBitmapMissing = true;
                                    }
                                }
                            }
                            else if (localIsDefined2())
                            {
                                PilotData localItem6() { object argIndex1 = pname; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                                localItem6().IsBitmapMissing = true;
                            }
                            else if (localIsDefined3())
                            {
                                NonPilotData localItem7() { object argIndex1 = pname; var ret = SRC.NPDList.Item(ref argIndex1); return ret; }

                                localItem7().IsBitmapMissing = true;
                            }
                        }
                    }
                }
                else
                {
                    My.MyProject.Forms.frmMessage.picFace.Image = Image.FromFile("");
                    My.MyProject.Forms.frmMessage.picFace.Refresh();
                    DisplayedPilot = "";
                    DisplayMode = "";
                }
            }

            // メッセージが空なら表示は止める
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            p = My.MyProject.Forms.frmMessage.picMessage;

            // メッセージウィンドウの状態を記録
            SaveMessageFormStatus();

            // メッセージを分割
            messages = new string[2];
            line_head = 1;
            buf = "";
            var loopTo = (short)Strings.Len(msg);
            for (i = 1; i <= loopTo; i++)
            {
                switch (Strings.Mid(msg, i, 1) ?? "")
                {
                    case ":":
                        {
                            buf = buf + Strings.Mid(msg, line_head, i - line_head);
                            messages[Information.UBound(messages)] = buf + ":";
                            Array.Resize(ref messages, Information.UBound(messages) + 1 + 1);
                            line_head = (short)(i + 1);
                            break;
                        }

                    case ";":
                        {
                            buf = buf + Strings.Mid(msg, line_head, i - line_head);
                            messages[Information.UBound(messages)] = buf;
                            buf = "";
                            Array.Resize(ref messages, Information.UBound(messages) + 1 + 1);
                            line_head = (short)(i + 1);
                            break;
                        }
                }
            }

            messages[Information.UBound(messages)] = buf + Strings.Mid(msg, line_head);
            wait_time = SRC.DEFAULT_LEVEL;

            // 強制改行の位置を設定
            string argoname = "改行時余白短縮";
            if (Expression.IsOptionDefined(ref argoname))
            {
                cl_margin[0] = 0.94f; // メッセージ長の超過による改行の位置
                cl_margin[1] = 0.7f; // "。"," "による改行の位置
                cl_margin[2] = 0.85f; // "、"による改行の位置
            }
            else
            {
                cl_margin[0] = 0.8f;
                cl_margin[1] = 0.6f;
                cl_margin[2] = 0.75f;
            }

            // 各メッセージを表示
            string fsuffix = default, fname0 = default, fpath = default;
            short first_id, last_id = default;
            int wait_time2;
            bool with_footer;
            var loopTo1 = (short)Information.UBound(messages);
            for (i = 1; i <= loopTo1; i++)
            {
                buf = messages[i];

                // メッセージ内の式置換を処理
                Event_Renamed.SaveBasePoint();
                Expression.FormatMessage(ref buf);
                Event_Renamed.RestoreBasePoint();

                // 特殊効果
                switch (Strings.LCase(Strings.Right(GeneralLib.LIndex(ref buf, 1), 4)) ?? "")
                {
                    case ".bmp":
                    case ".jpg":
                    case ".gif":
                    case ".png":
                        {

                            // 右ボタンを押されていたらスキップ
                            if (IsRButtonPressed())
                            {
                                goto NextMessage;
                            }

                            // カットインの表示
                            fname = GeneralLib.LIndex(ref buf, 1);

                            // アニメ指定かどうか判定
                            j = (short)Strings.InStr(fname, "[");
                            if (j > 0 & Strings.InStr(fname, "].") == Strings.Len(fname) - 4)
                            {
                                fname0 = Strings.Left(fname, j - 1);
                                fsuffix = Strings.Right(fname, 4);
                                buf2 = Strings.Mid(fname, j + 1, Strings.Len(fname) - j - 5);
                                j = (short)Strings.InStr(buf2, "-");
                                first_id = Conversions.ToShort(Strings.Left(buf2, j - 1));
                                last_id = Conversions.ToShort(Strings.Mid(buf2, j + 1));
                            }
                            else
                            {
                                first_id = -1;
                            }

                            // 画像表示のオプション
                            options = "";
                            n = GeneralLib.LLength(ref buf);
                            j = 2;
                            opt_n = 2;
                            while (j <= n)
                            {
                                buf2 = GeneralLib.LIndex(ref buf, j);
                                switch (buf2 ?? "")
                                {
                                    case "透過":
                                    case "背景":
                                    case "白黒":
                                    case "セピア":
                                    case "明":
                                    case "暗":
                                    case "上下反転":
                                    case "左右反転":
                                    case "上半分":
                                    case "下半分":
                                    case "右半分":
                                    case "左半分":
                                    case "右上":
                                    case "左上":
                                    case "右下":
                                    case "左下":
                                    case "ネガポジ反転":
                                    case "シルエット":
                                    case "夕焼け":
                                    case "水中":
                                    case "保持":
                                        {
                                            options = options + buf2 + " ";
                                            break;
                                        }

                                    case "消去":
                                        {
                                            clear_every_time = true;
                                            break;
                                        }

                                    case "右回転":
                                        {
                                            j = (short)(j + 1);
                                            options = options + "右回転 " + GeneralLib.LIndex(ref buf, j) + " ";
                                            break;
                                        }

                                    case "左回転":
                                        {
                                            j = (short)(j + 1);
                                            options = options + "左回転 " + GeneralLib.LIndex(ref buf, j) + " ";
                                            break;
                                        }

                                    case "-":
                                        {
                                            // スキップ
                                            opt_n = (short)(j + 1);
                                            break;
                                        }

                                    default:
                                        {
                                            if (Strings.Asc(buf2) == 35 & Strings.Len(buf2) == 7)
                                            {
                                                // 透過色設定
                                                cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                                                StringType.MidStmtStr(ref cname, 1, 2, "&H");
                                                var midTmp = Strings.Mid(buf2, 6, 2);
                                                StringType.MidStmtStr(ref cname, 3, 2, midTmp);
                                                var midTmp1 = Strings.Mid(buf2, 4, 2);
                                                StringType.MidStmtStr(ref cname, 5, 2, midTmp1);
                                                var midTmp2 = Strings.Mid(buf2, 2, 2);
                                                StringType.MidStmtStr(ref cname, 7, 2, midTmp2);
                                                if (Information.IsNumeric(cname))
                                                {
                                                    if (Conversions.ToInteger(cname) != ColorTranslator.ToOle(Color.White))
                                                    {
                                                        options = options + SrcFormatter.Format(Conversions.ToInteger(cname)) + " ";
                                                    }
                                                }
                                            }
                                            else if (Information.IsNumeric(buf2))
                                            {
                                                // スキップ
                                                opt_n = (short)(j + 1);
                                            }

                                            break;
                                        }
                                }

                                j = (short)(j + 1);
                            }

                            if (Strings.Asc(fname) == 64) // @
                            {
                                // パイロット画像切り替えの場合

                                if (first_id == -1)
                                {
                                    fname = Strings.Mid(fname, 2);
                                }
                                else
                                {
                                    fname0 = Strings.Mid(fname0, 2);
                                    fname = fname0 + SrcFormatter.Format(first_id, "00") + fsuffix;
                                }

                                // ウィンドウが表示されていなければ表示
                                if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    Unit argu1 = null;
                                    Unit argu2 = null;
                                    OpenMessageForm(u1: ref argu1, u2: ref argu2);
                                }

                                if (wait_time > 0)
                                {
                                    start_time = GeneralLib.timeGetTime();
                                }

                                // 画像表示のオプション
                                options = options + " メッセージ";
                                switch (Map.MapDrawMode ?? "")
                                {
                                    case "セピア":
                                    case "白黒":
                                        {
                                            options = options + " " + Map.MapDrawMode;
                                            break;
                                        }
                                }

                                if (first_id == -1)
                                {
                                    // １枚画像の場合
                                    DrawPicture(ref fname, 0, 0, 64, 64, 0, 0, 0, 0, ref options);
                                    My.MyProject.Forms.frmMessage.picFace.Refresh();
                                    if (wait_time > 0)
                                    {
                                        while (start_time + wait_time > GeneralLib.timeGetTime())
                                            Sleep(20);
                                    }
                                }
                                else
                                {
                                    // アニメーションの場合
                                    var loopTo2 = last_id;
                                    for (j = first_id; j <= loopTo2; j++)
                                    {
                                        fname = fpath + fname0 + SrcFormatter.Format(j, "00") + fsuffix;
                                        DrawPicture(ref fname, 0, 0, 64, 64, 0, 0, 0, 0, ref options);
                                        My.MyProject.Forms.frmMessage.picFace.Refresh();
                                        if (wait_time > 0)
                                        {
                                            wait_time2 = wait_time * (j - first_id + 1) / (last_id - first_id);
                                            cur_time = GeneralLib.timeGetTime();
                                            if (cur_time < start_time + wait_time2)
                                            {
                                                Sleep(start_time + wait_time2 - cur_time);
                                            }
                                        }
                                    }
                                }

                                wait_time = SRC.DEFAULT_LEVEL;
                                DisplayedPilot = fname;
                                goto NextMessage;
                            }

                            // 表示画像のサイズ
                            if (opt_n > 2)
                            {
                                buf2 = GeneralLib.LIndex(ref buf, 2);
                                if (buf2 == "-")
                                {
                                    dw = SRC.DEFAULT_LEVEL.ToString();
                                }
                                else
                                {
                                    dw = GeneralLib.StrToLng(ref buf2).ToString();
                                }

                                buf2 = GeneralLib.LIndex(ref buf, 3);
                                if (buf2 == "-")
                                {
                                    dh = SRC.DEFAULT_LEVEL.ToString();
                                }
                                else
                                {
                                    dh = GeneralLib.StrToLng(ref buf2).ToString();
                                }
                            }
                            else
                            {
                                dw = SRC.DEFAULT_LEVEL.ToString();
                                dh = SRC.DEFAULT_LEVEL.ToString();
                            }

                            // 表示画像の位置
                            if (opt_n > 4)
                            {
                                buf2 = GeneralLib.LIndex(ref buf, 4);
                                if (buf2 == "-")
                                {
                                    dx = SRC.DEFAULT_LEVEL.ToString();
                                }
                                else
                                {
                                    dx = GeneralLib.StrToLng(ref buf2).ToString();
                                }

                                buf2 = GeneralLib.LIndex(ref buf, 5);
                                if (buf2 == "-")
                                {
                                    dy = SRC.DEFAULT_LEVEL.ToString();
                                }
                                else
                                {
                                    dy = GeneralLib.StrToLng(ref buf2).ToString();
                                }
                            }
                            else
                            {
                                dx = SRC.DEFAULT_LEVEL.ToString();
                                dy = SRC.DEFAULT_LEVEL.ToString();
                            }

                            if (wait_time > 0)
                            {
                                start_time = GeneralLib.timeGetTime();
                            }

                            if (first_id == -1)
                            {
                                // １枚絵の場合
                                if (clear_every_time)
                                {
                                    ClearPicture();
                                }

                                DrawPicture(ref fname, Conversions.ToInteger(dx), Conversions.ToInteger(dy), Conversions.ToInteger(dw), Conversions.ToInteger(dh), 0, 0, 0, 0, ref options);
                                need_refresh = true;
                                if (wait_time > 0)
                                {
                                    MainForm.picMain(0).Refresh();
                                    need_refresh = false;
                                    cur_time = GeneralLib.timeGetTime();
                                    if (cur_time < start_time + wait_time)
                                    {
                                        Sleep(start_time + wait_time - cur_time);
                                    }

                                    wait_time = SRC.DEFAULT_LEVEL;
                                }
                            }
                            else
                            {
                                // アニメーションの場合
                                var loopTo3 = last_id;
                                for (j = first_id; j <= loopTo3; j++)
                                {
                                    fname = fname0 + SrcFormatter.Format(j, "00") + fsuffix;
                                    if (clear_every_time)
                                    {
                                        ClearPicture();
                                    }

                                    DrawPicture(ref fname, Conversions.ToInteger(dx), Conversions.ToInteger(dy), Conversions.ToInteger(dw), Conversions.ToInteger(dh), 0, 0, 0, 0, ref options);

                                    MainForm.picMain(0).Refresh();
                                    if (wait_time > 0)
                                    {
                                        wait_time2 = wait_time * (j - first_id + 1) / (last_id - first_id);
                                        cur_time = GeneralLib.timeGetTime();
                                        if (cur_time < start_time + wait_time2)
                                        {
                                            Sleep(start_time + wait_time2 - cur_time);
                                        }
                                    }
                                }

                                wait_time = SRC.DEFAULT_LEVEL;
                            }

                            goto NextMessage;
                            break;
                        }

                    case ".wav":
                    case ".mp3":
                        {
                            // 右ボタンを押されていたらスキップ
                            if (IsRButtonPressed())
                            {
                                goto NextMessage;
                            }

                            // 効果音の演奏
                            Sound.PlayWave(ref buf);
                            if (wait_time > 0)
                            {
                                if (need_refresh)
                                {
                                    MainForm.picMain(0).Refresh();
                                    need_refresh = false;
                                }

                                Sleep(wait_time);
                                wait_time = SRC.DEFAULT_LEVEL;
                            }

                            goto NextMessage;
                            break;
                        }
                }

                // 戦闘アニメ呼び出し
                if (Strings.Left(buf, 1) == "@")
                {
                    string arganame = Strings.Mid(buf, 2);
                    Effect.ShowAnimation(ref arganame);
                    goto NextMessage;
                }

                // 特殊コマンド
                switch (Strings.LCase(GeneralLib.LIndex(ref buf, 1)) ?? "")
                {
                    case "clear":
                        {
                            // カットインの消去
                            ClearPicture();
                            need_refresh = true;
                            goto NextMessage;
                            break;
                        }

                    case "keep":
                        {
                            // カットインの保存
                            IsPictureDrawn = false;
                            goto NextMessage;
                            break;
                        }
                }

                // ウェイト
                if (Information.IsNumeric(buf))
                {
                    wait_time = (int)(100d * Conversions.ToDouble(buf));
                    goto NextMessage;
                }

                // これよりメッセージの表示

                // 空メッセージの場合は表示しない
                if (string.IsNullOrEmpty(buf))
                {
                    goto NextMessage;
                }

                // メッセージウィンドウの状態が変化している場合は復元
                KeepMessageFormStatus();
                // ウィンドウをクリア
                p.Cls();
                p.CurrentX = 1;

                // フォント設定を初期化
                p.Font = SrcFormatter.FontChangeBold(p.Font, false);
                p.Font = SrcFormatter.FontChangeItalic(p.Font, false);
                p.Font = SrcFormatter.FontChangeName(p.Font, "ＭＳ Ｐ明朝");
                p.Font = SrcFormatter.FontChangeSize(p.Font, 12f);
                p.ForeColor = Color.Black;

                // 話者名と括弧の表示処理
                is_char_message = false;
                if (pname != "システム" & (!string.IsNullOrEmpty(pname) & pname != "-" | Strings.Left(buf, 1) == "「" & Strings.Right(buf, 1) == "」" | Strings.Left(buf, 1) == "『" & Strings.Right(buf, 1) == "』"))
                {
                    is_char_message = true;

                    // 話者のグラフィックを表示
                    if (pname == "-" & Commands.SelectedUnit is object)
                    {
                        if (Commands.SelectedUnit.CountPilot() > 0)
                        {
                            fname = Commands.SelectedUnit.MainPilot().get_Bitmap(false);
                            string argdraw_option1 = "メッセージ " + msg_mode;
                            if (DrawPicture(ref fname, 0, 0, 64, 64, 0, 0, 0, 0, ref argdraw_option1))
                            {
                                My.MyProject.Forms.frmMessage.picFace.Refresh();
                                DisplayedPilot = fname;
                                DisplayMode = msg_mode;
                            }
                        }
                    }

                    // 話者名を表示
                    if (string.IsNullOrEmpty(pnickname) & pname == "-" & Commands.SelectedUnit is object)
                    {
                        if (Commands.SelectedUnit.CountPilot() > 0)
                        {
                            p.Print(Commands.SelectedUnit.MainPilot().get_Nickname(false));
                        }
                    }
                    else if (!string.IsNullOrEmpty(pnickname))
                    {
                        p.Print(pnickname);
                    }

                    // メッセージが途中で終わっているか判定
                    if (Strings.Right(buf, 1) != ":")
                    {
                        with_footer = true;
                    }
                    else
                    {
                        with_footer = false;
                        prev_lnum = lnum;
                        buf = Strings.Left(buf, Strings.Len(buf) - 1);
                    }

                    // 括弧を付加
                    if ((Strings.Left(buf, 1) == "(" | Strings.Left(buf, 1) == "（") & (!with_footer | Strings.Right(buf, 1) == ")" | Strings.Right(buf, 1) == "）"))
                    {
                        // モノローグ
                        if (with_footer)
                        {
                            buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                            buf = "（" + buf + "）";
                        }
                        else
                        {
                            buf = Strings.Mid(buf, 2);
                            buf = "（" + buf;
                        }
                    }
                    else if (Strings.Left(buf, 1) == "「" & (!with_footer | Strings.Right(buf, 1) == "」"))
                    {
                    }
                    // 「」の括弧が既にあるので変更しない
                    else if (Strings.Left(buf, 1) == "『" & (!with_footer | Strings.Right(buf, 1) == "』"))
                    {
                    }
                    // 『』の括弧が既にあるので変更しない
                    else if (with_footer)
                    {
                        buf = "「" + buf + "」";
                    }
                    else
                    {
                        buf = "「" + buf;
                    }
                }
                // メッセージが途中で終わっているか判定
                else if (Strings.Right(buf, 1) == ":")
                {
                    prev_lnum = lnum;
                    buf = Strings.Left(buf, Strings.Len(buf) - 1);
                }

                prev_lnum = (short)GeneralLib.MaxLng(prev_lnum, 1);
                lnum = 0;
                line_head = 1;
                var loopTo4 = (short)Strings.Len(buf);
                for (j = 1; j <= loopTo4; j++)
                {
                    ch = Strings.Mid(buf, j, 1);

                    // 「.」の場合は必ず改行
                    if (ch == ".")
                    {
                        if (j != line_head)
                        {
                            string argmsg = Strings.Mid(buf, line_head, j - line_head);
                            PrintMessage(ref argmsg, !is_char_message);
                            if (is_char_message)
                            {
                                p.Print("  ");
                            }

                            lnum = (short)(lnum + 1);
                        }

                        line_head = (short)(j + 1);
                        goto NextLoop;
                    }

                    // タグ内では改行しない
                    if (ch == "<")
                    {
                        in_tag = true;
                        goto NextLoop;
                    }
                    else if (ch == ">")
                    {
                        in_tag = false;
                    }
                    else if (in_tag)
                    {
                        goto NextLoop;
                    }

                    // メッセージが途切れてしまう場合は必ず改行
                    if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > 0.95d * SrcFormatter.PixelsToTwipsX(p.Width))
                    {
                        string argmsg1 = Strings.Mid(buf, line_head, j - line_head + 1);
                        PrintMessage(ref argmsg1, !is_char_message);
                        if (is_char_message)
                        {
                            p.Print("  ");
                        }

                        line_head = (short)(j + 1);
                        lnum = (short)(lnum + 1);
                        goto NextLoop;
                    }

                    // 禁則処理
                    switch (Strings.Mid(buf, j + 1, 1) ?? "")
                    {
                        case "。":
                        case "、":
                        case "…":
                        case "‥":
                        case "・":
                        case "･":
                        case "～":
                        case "ー":
                        case "－":
                        case "！":
                        case "？":
                        case "」":
                        case "』":
                        case "）":
                        case ")":
                        case " ":
                        case ".":
                            {
                                goto NextLoop;
                                break;
                            }
                    }

                    switch (Strings.Mid(buf, j + 2, 1) ?? "")
                    {
                        case "。":
                        case "、":
                        case "…":
                        case "‥":
                        case "・":
                        case "･":
                        case "～":
                        case "ー":
                        case "－":
                        case "！":
                        case "？":
                        case "」":
                        case "』":
                        case "）":
                        case ")":
                        case " ":
                        case ".":
                            {
                                goto NextLoop;
                                break;
                            }
                    }

                    if (Strings.Mid(buf, j + 3, 1) == ".")
                    {
                        goto NextLoop;
                    }

                    // 改行の判定
                    if (MessageLen(Strings.Mid(messages[i], line_head)) < 0.95d * SrcFormatter.PixelsToTwipsX(p.Width))
                    {
                        // 全体が一行に収まる場合
                        goto NextLoop;
                    }

                    switch (ch ?? "")
                    {
                        case "。":
                            {
                                if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[1] * SrcFormatter.PixelsToTwipsX(p.Width))
                                {
                                    string argmsg2 = Strings.Mid(buf, line_head, j - line_head + 1);
                                    PrintMessage(ref argmsg2, !is_char_message);
                                    if (is_char_message)
                                    {
                                        p.Print("  ");
                                    }

                                    line_head = (short)(j + 1);
                                    lnum = (short)(lnum + 1);
                                }

                                break;
                            }

                        case " ":
                            {
                                if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[1] * SrcFormatter.PixelsToTwipsX(p.Width))
                                {
                                    string argmsg3 = Strings.Mid(buf, line_head, j - line_head);
                                    PrintMessage(ref argmsg3, !is_char_message);
                                    if (is_char_message)
                                    {
                                        p.Print("  ");
                                    }

                                    line_head = (short)(j + 1);
                                    lnum = (short)(lnum + 1);
                                }

                                break;
                            }

                        case "、":
                            {
                                if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[2] * SrcFormatter.PixelsToTwipsX(p.Width))
                                {
                                    string argmsg4 = Strings.Mid(buf, line_head, j - line_head + 1);
                                    PrintMessage(ref argmsg4, !is_char_message);
                                    if (is_char_message)
                                    {
                                        p.Print("  ");
                                    }

                                    line_head = (short)(j + 1);
                                    lnum = (short)(lnum + 1);
                                }

                                break;
                            }

                        default:
                            {
                                if (MessageLen(Strings.Mid(buf, line_head, j - line_head)) > cl_margin[0] * SrcFormatter.PixelsToTwipsX(p.Width))
                                {
                                    string argmsg5 = Strings.Mid(buf, line_head, j - line_head);
                                    PrintMessage(ref argmsg5, !is_char_message);
                                    if (is_char_message)
                                    {
                                        p.Print("  ");
                                    }

                                    line_head = j;
                                    lnum = (short)(lnum + 1);
                                }

                                break;
                            }
                    }

                    NextLoop:
                    ;
                }
                // メッセージの残りを表示しておく
                if (Strings.Len(buf) >= line_head)
                {
                    string argmsg6 = Strings.Mid(buf, line_head);
                    PrintMessage(ref argmsg6, !is_char_message);
                    lnum = (short)(lnum + 1);
                }

                // フォント設定を元に戻す
                p.Font = SrcFormatter.FontChangeBold(p.Font, false);
                p.Font = SrcFormatter.FontChangeItalic(p.Font, false);
                p.Font = SrcFormatter.FontChangeName(p.Font, "ＭＳ Ｐ明朝");
                p.Font = SrcFormatter.FontChangeSize(p.Font, 12f);
                p.ForeColor = Color.Black;

                // デフォルトのウェイト
                if (wait_time == SRC.DEFAULT_LEVEL)
                {
                    wait_time = (lnum - prev_lnum + 1) * MessageWait;
                    if (msg_mode == "高速")
                    {
                        wait_time = wait_time / 2;
                    }
                }

                // 画面を更新
                if (need_refresh)
                {
                    MainForm.picMain(0).Refresh();
                    need_refresh = false;
                }

                Application.DoEvents();

                // 待ち時間が切れるまで待機
                start_time = GeneralLib.timeGetTime();
                IsFormClicked = false;
                while (start_time + wait_time > GeneralLib.timeGetTime())
                {
                    // 左ボタンが押されたらメッセージ送り
                    if (IsFormClicked)
                    {
                        break;
                    }

                    // 右ボタンを押されていたら早送り
                    if (IsRButtonPressed())
                    {
                        break;
                    }

                    Sleep(20);
                    Application.DoEvents();
                }

                wait_time = SRC.DEFAULT_LEVEL;
                NextMessage:
                ;
            }

            // 戦闘アニメデータのカットイン表示？
            if (pname == "-")
            {
                return;
            }

            // 画面を更新
            if (need_refresh)
            {
                MainForm.picMain(0).Refresh();
                need_refresh = false;
            }

            // メッセージデータの最後にウェイトの指定が行われていた場合
            if (wait_time > 0)
            {
                start_time = GeneralLib.timeGetTime();
                IsFormClicked = false;
                while (start_time + wait_time > GeneralLib.timeGetTime())
                {
                    // 左ボタンが押されたらメッセージ送り
                    if (IsFormClicked)
                    {
                        break;
                    }

                    // 右ボタンを押されていたら早送り
                    if (IsRButtonPressed())
                    {
                        break;
                    }

                    Sleep(20);
                    Application.DoEvents();
                }
            }

            // メッセージウィンドウの状態が変化している場合は復元
            KeepMessageFormStatus();
            Application.DoEvents();
            return;
            ErrorHandler:
            ;
            string argmsg7 = "画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。";
            ErrorMessage(ref argmsg7);
        }

        // システムによるメッセージを表示
        public static void DisplaySysMessage(string msg, bool short_wait = false)
        {
            short j, i, line_head;
            string ch, buf;
            PictureBox p;
            short lnum;
            int start_time, wait_time;
            var in_tag = default(bool);

            // メッセージウィンドウが表示されていない場合は表示をキャンセル
            if ((int)My.MyProject.Forms.frmMessage.WindowState == 1)
            {
                return;
            }

            // メッセージ内の式を置換
            Expression.FormatMessage(ref msg);

            // ウィンドウのキャプションを設定
            if (My.MyProject.Forms.frmMessage.Text == "メッセージ (自動送り)")
            {
                My.MyProject.Forms.frmMessage.Text = "メッセージ";
            }

            p = My.MyProject.Forms.frmMessage.picMessage;
            // メッセージウィンドウをクリア
            p.Cls();
            p.CurrentX = 1;

            // フォント設定を初期化
            p.Font = SrcFormatter.FontChangeBold(p.Font, false);
            p.Font = SrcFormatter.FontChangeItalic(p.Font, false);
            p.Font = SrcFormatter.FontChangeName(p.Font, "ＭＳ Ｐ明朝");
            p.Font = SrcFormatter.FontChangeSize(p.Font, 12f);
            p.ForeColor = Color.Black;

            // メッセージを表示
            lnum = 0;
            line_head = 1;
            var loopTo = (short)Strings.Len(msg);
            for (i = 1; i <= loopTo; i++)
            {
                ch = Strings.Mid(msg, i, 1);

                // 「;」の場合は必ず改行
                if (ch == ";")
                {
                    if (line_head != i)
                    {
                        buf = Strings.Mid(msg, line_head, i - line_head);
                        PrintMessage(ref buf, true);
                        lnum = (short)(lnum + 1);
                    }

                    line_head = (short)(i + 1);
                    goto NextLoop;
                }

                // タグ内では改行しない
                if (ch == "<")
                {
                    in_tag = true;
                    goto NextLoop;
                }
                else if (ch == ">")
                {
                    in_tag = false;
                }
                else if (in_tag)
                {
                    goto NextLoop;
                }

                // 禁則処理
                if (ch == "。" | ch == "、")
                {
                    goto NextLoop;
                }

                if (i < Strings.Len(msg))
                {
                    if (Strings.Mid(msg, i + 1, 1) == "。" | Strings.Mid(msg, i + 1, 1) == "、")
                    {
                        goto NextLoop;
                    }
                }

                if (MessageLen(Strings.Mid(msg, line_head)) < SrcFormatter.PixelsToTwipsX(p.Width))
                {
                    // 全体が一行に収まる場合
                    goto NextLoop;
                }

                if (GeneralLib.IsSpace(ref ch) & MessageLen(Strings.Mid(msg, line_head, i - line_head)) > 0.5d * SrcFormatter.PixelsToTwipsX(p.Width))
                {
                    buf = Strings.Mid(msg, line_head, i - line_head);
                    PrintMessage(ref buf, true);
                    lnum = (short)(lnum + 1);
                    line_head = (short)(i + 1);
                }
                else if (MessageLen(Strings.Mid(msg, line_head, i - line_head + 1)) > 0.95d * SrcFormatter.PixelsToTwipsX(p.Width))
                {
                    buf = Strings.Mid(msg, line_head, i - line_head + 1);
                    PrintMessage(ref buf, true);
                    lnum = (short)(lnum + 1);
                    line_head = (short)(i + 1);
                }
                else if (ch == "[")
                {
                    // []で囲まれた文字列内では改行しない
                    var loopTo1 = (short)Strings.Len(msg);
                    for (j = i; j <= loopTo1; j++)
                    {
                        if (Strings.Mid(msg, j, 1) == "]")
                        {
                            break;
                        }
                    }

                    if (MessageLen(Strings.Mid(msg, line_head, j - line_head)) > 0.95d * SrcFormatter.PixelsToTwipsX(p.Width))
                    {
                        buf = Strings.Mid(msg, line_head, i - line_head);
                        PrintMessage(ref buf, true);
                        lnum = (short)(lnum + 1);
                        line_head = i;
                    }
                }

                NextLoop:
                ;
            }

            buf = Strings.Mid(msg, line_head);
            PrintMessage(ref buf, true);
            lnum = (short)(lnum + 1);

            // フォント設定を元に戻す
            p.Font = SrcFormatter.FontChangeBold(p.Font, false);
            p.Font = SrcFormatter.FontChangeItalic(p.Font, false);
            p.Font = SrcFormatter.FontChangeName(p.Font, "ＭＳ Ｐ明朝");
            p.Font = SrcFormatter.FontChangeSize(p.Font, 12f);
            p.ForeColor = Color.Black;

            // ウェイトを計算
            wait_time = (int)((0.8d + 0.5d * lnum) * MessageWait);
            if (short_wait)
            {
                wait_time = wait_time / 2;
            }

            Application.DoEvents();

            // 待ち時間が切れるまで待機
            IsFormClicked = false;
            start_time = GeneralLib.timeGetTime();
            while (start_time + wait_time > GeneralLib.timeGetTime())
            {
                // 左ボタンが押されたらメッセージ送り
                if (IsFormClicked)
                {
                    break;
                }

                // 右ボタンを押されていたら早送り
                if (IsRButtonPressed())
                {
                    break;
                }

                Sleep(20);
                Application.DoEvents();
            }
        }


        // === マップウィンドウに関する処理 ===

        // マップ画面背景の設定
        public static void SetupBackground([Optional, DefaultParameterValue("")] ref string draw_mode, [Optional, DefaultParameterValue("")] ref string draw_option, [Optional, DefaultParameterValue(0)] ref int filter_color, [Optional, DefaultParameterValue(0d)] ref double filter_trans_par)
        {
            var B = default(object);
            short k, i, j, ret;
            short xx, yy;
            short terrain_bmp_count;
            string[] terrain_bmp_type;
            short[] terrain_bmp_num;
            short[] terrain_bmp_x;
            short[] terrain_bmp_y;
            string fname;
            Map.IsMapDirty = false;
            IsPictureVisible = false;
            IsCursorVisible = false;

            // ユニット画像色を変更しないといけない場合
            if ((Map.MapDrawMode ?? "") != (draw_mode ?? ""))
            {
                SRC.UList.ClearUnitBitmap();
                Map.MapDrawMode = draw_mode;
                Map.MapDrawFilterColor = filter_color;
                Map.MapDrawFilterTransPercent = filter_trans_par;
            }
            else if (draw_mode == "フィルタ" & (Map.MapDrawFilterColor != filter_color | Map.MapDrawFilterTransPercent != filter_trans_par))
            {
                SRC.UList.ClearUnitBitmap();
                Map.MapDrawMode = draw_mode;
                Map.MapDrawFilterColor = filter_color;
                Map.MapDrawFilterTransPercent = filter_trans_par;
            }

            // マップ背景の設定
            {
                var withBlock = MainForm;
                switch (draw_option ?? "")
                {
                    case "ステータス":
                        {
                            {
                                var withBlock1 = withBlock.picBack;
                                ret = (short)PatBlt(withBlock1.hDC, 0, 0, withBlock1.width, withBlock1.Height, BLACKNESS);
                            }

                            return;
                        }

                    default:
                        {
                            MapX = (short)(MainWidth / 2 + 1);
                            MapY = (short)(MainHeight / 2 + 1);
                            break;
                        }
                }

                // 各マスのマップ画像を表示
                var loopTo = Map.MapWidth;
                for (i = (short)1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (j = (short)1; j <= loopTo1; j++)
                    {
                        xx = (short)(32 * ((int)i - 1));
                        yy = (short)(32 * ((int)j - 1));

                        // DEL START 240a
                        // 'マップ画像が既に読み込まれているか判定
                        // For k = 1 To terrain_bmp_count
                        // If terrain_bmp_type(k) = MapData(i, j, 0) _
                        // '                        And terrain_bmp_num(k) = MapData(i, j, 1) _
                        // '                    Then
                        // Exit For
                        // End If
                        // Next

                        // If k <= terrain_bmp_count Then
                        // '既に描画済みの画像は描画した個所から転送
                        // ret = BitBlt(.picBack.hDC, _
                        // '                        xx, yy, 32, 32, _
                        // '                        .picBack.hDC, terrain_bmp_x(k), terrain_bmp_y(k), SRCCOPY)
                        // MapImageFileTypeData(i, j) = _
                        // '                        MapImageFileTypeData(terrain_bmp_x(k) \ 32 + 1, terrain_bmp_y(k) \ 32 + 1)
                        // Else
                        // '新規の画像の場合
                        // DEL  END  240a
                        // 画像ファイルを探す
                        // MOD START 240a
                        // fname = SearchTerrainImageFile(MapData(i, j, 0), MapData(i, j, 1), i, j)
                        fname = Map.SearchTerrainImageFile(Map.MapData[i, j, Map.MapDataIndex.TerrainType], Map.MapData[i, j, Map.MapDataIndex.BitmapNo], i, j);
                        // MOD  END  240a

                        // 画像を取り込み
                        if (!string.IsNullOrEmpty(fname))
                        {
                            ;
                            withBlock.picTmp32(0) = Image.FromFile(fname);
                            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
                            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToZeroStatement not implemented, please report this issue in 'On Error GoTo 0' at character 132060


                            Input:
                                                    On Error GoTo 0

                             */
                        }
                        else
                        {
                            ret = (short)PatBlt(withBlock.picTmp32(0).hDC, 0, 0, 32, 32, BLACKNESS);
                        }

                        // マップ設定によって表示色を変更
                        switch (draw_mode ?? "")
                        {
                            case "夜":
                                {
                                    var argpic = withBlock.picTmp32(0);
                                    Graphics.GetImage(ref argpic);
                                    Graphics.Dark();
                                    var argpic1 = withBlock.picTmp32(0);
                                    Graphics.SetImage(ref argpic1);
                                    break;
                                }

                            case "セピア":
                                {
                                    var argpic2 = withBlock.picTmp32(0);
                                    Graphics.GetImage(ref argpic2);
                                    Graphics.Sepia();
                                    var argpic3 = withBlock.picTmp32(0);
                                    Graphics.SetImage(ref argpic3);
                                    break;
                                }

                            case "白黒":
                                {
                                    var argpic4 = withBlock.picTmp32(0);
                                    Graphics.GetImage(ref argpic4);
                                    Graphics.Monotone();
                                    var argpic5 = withBlock.picTmp32(0);
                                    Graphics.SetImage(ref argpic5);
                                    break;
                                }

                            case "夕焼け":
                                {
                                    var argpic6 = withBlock.picTmp32(0);
                                    Graphics.GetImage(ref argpic6);
                                    Graphics.Sunset();
                                    var argpic7 = withBlock.picTmp32(0);
                                    Graphics.SetImage(ref argpic7);
                                    break;
                                }

                            case "水中":
                                {
                                    var argpic8 = withBlock.picTmp32(0);
                                    Graphics.GetImage(ref argpic8);
                                    Graphics.Water();
                                    var argpic9 = withBlock.picTmp32(0);
                                    Graphics.SetImage(ref argpic9);
                                    break;
                                }

                            case "フィルタ":
                                {
                                    var argpic10 = withBlock.picTmp32(0);
                                    Graphics.GetImage(ref argpic10);
                                    Graphics.ColorFilter(ref Map.MapDrawFilterColor, ref Map.MapDrawFilterTransPercent);
                                    var argpic11 = withBlock.picTmp32(0);
                                    Graphics.SetImage(ref argpic11);
                                    break;
                                }
                        }

                        // 画像を描き込み
                        ret = (short)BitBlt(withBlock.picBack.hDC, xx, yy, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, SRCCOPY);
                        // DEL START 240a
                        // '画像を登録
                        // terrain_bmp_count = terrain_bmp_count + 1
                        // ReDim Preserve terrain_bmp_type(terrain_bmp_count)
                        // ReDim Preserve terrain_bmp_num(terrain_bmp_count)
                        // ReDim Preserve terrain_bmp_x(terrain_bmp_count)
                        // ReDim Preserve terrain_bmp_y(terrain_bmp_count)
                        // terrain_bmp_type(terrain_bmp_count) = MapData(i, j, 0)
                        // terrain_bmp_num(terrain_bmp_count) = MapData(i, j, 1)
                        // terrain_bmp_x(terrain_bmp_count) = xx
                        // terrain_bmp_y(terrain_bmp_count) = yy
                        // End If
                        // DEL  END  240a
                        // ADD START 240a
                        // レイヤー描画する必要がある場合は描画する
                        if ((int)Map.BoxTypes.Upper == (int)Map.MapData[i, j, Map.MapDataIndex.BoxType] | (int)Map.BoxTypes.UpperBmpOnly == (int)Map.MapData[i, j, Map.MapDataIndex.BoxType])
                        {
                            // 画像ファイルを探す
                            fname = Map.SearchTerrainImageFile(Map.MapData[i, j, Map.MapDataIndex.LayerType], Map.MapData[i, j, Map.MapDataIndex.LayerBitmapNo], i, j);

                            // 画像を取り込み
                            if (!string.IsNullOrEmpty(fname))
                            {
                                ;
                                withBlock.picTmp32(0) = Image.FromFile(fname);
                                ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
                                /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToZeroStatement not implemented, please report this issue in 'On Error GoTo 0' at character 137805


                                Input:
                                                            On Error GoTo 0

                                 */
                                BGColor = ColorTranslator.ToOle(Color.White);
                                // マップ設定によって表示色を変更
                                switch (draw_mode ?? "")
                                {
                                    case "夜":
                                        {
                                            var argpic12 = withBlock.picTmp32(0);
                                            Graphics.GetImage(ref argpic12);
                                            Graphics.Dark(true);
                                            var argpic13 = withBlock.picTmp32(0);
                                            Graphics.SetImage(ref argpic13);
                                            break;
                                        }

                                    case "セピア":
                                        {
                                            var argpic14 = withBlock.picTmp32(0);
                                            Graphics.GetImage(ref argpic14);
                                            Graphics.Sepia(true);
                                            var argpic15 = withBlock.picTmp32(0);
                                            Graphics.SetImage(ref argpic15);
                                            break;
                                        }

                                    case "白黒":
                                        {
                                            var argpic16 = withBlock.picTmp32(0);
                                            Graphics.GetImage(ref argpic16);
                                            Graphics.Monotone(true);
                                            var argpic17 = withBlock.picTmp32(0);
                                            Graphics.SetImage(ref argpic17);
                                            break;
                                        }

                                    case "夕焼け":
                                        {
                                            var argpic18 = withBlock.picTmp32(0);
                                            Graphics.GetImage(ref argpic18);
                                            Graphics.Sunset(true);
                                            var argpic19 = withBlock.picTmp32(0);
                                            Graphics.SetImage(ref argpic19);
                                            break;
                                        }

                                    case "水中":
                                        {
                                            var argpic20 = withBlock.picTmp32(0);
                                            Graphics.GetImage(ref argpic20);
                                            Graphics.Water(true);
                                            var argpic21 = withBlock.picTmp32(0);
                                            Graphics.SetImage(ref argpic21);
                                            break;
                                        }

                                    case "フィルタ":
                                        {
                                            var argpic22 = withBlock.picTmp32(0);
                                            Graphics.GetImage(ref argpic22);
                                            Graphics.ColorFilter(ref Map.MapDrawFilterColor, ref Map.MapDrawFilterTransPercent, true);
                                            var argpic23 = withBlock.picTmp32(0);
                                            Graphics.SetImage(ref argpic23);
                                            break;
                                        }
                                }

                                // 画像を透過描き込み
                                ret = (short)TransparentBlt(withBlock.picBack.hDC, xx, yy, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, 32, 32, BGColor);
                            }
                        }
                        // ADD  END  240a
                    }
                }
                // MapDrawn:  '使用されていないラベルなので削除

                // マス目の表示
                if (SRC.ShowSquareLine)
                {
                    MainForm.picBack.Line((0, 0) - (MapPWidth - 1, MapPHeight - 1), Information.RGB(100, 100, 100), B);
                    var loopTo2 = (short)(Map.MapWidth - 1);
                    for (i = 1; i <= loopTo2; i++)
                        withBlock.picBack.Line((32 * i, -1) - (32 * i, MapPHeight), Information.RGB(100, 100, 100));
                    var loopTo3 = (short)(Map.MapHeight - 1);
                    for (i = 1; i <= loopTo3; i++)
                        withBlock.picBack.Line((0, 32 * i - 1) - (MapPWidth, 32 * i - 1), Information.RGB(100, 100, 100));
                }

                // マスク入り背景画面を作成
                ret = (short)BitBlt(withBlock.picMaskedBack.hDC, 0, 0, MapPWidth, MapPHeight, withBlock.picBack.hDC, 0, 0, SRCCOPY);
                var loopTo4 = Map.MapWidth;
                for (i = 1; i <= loopTo4; i++)
                {
                    var loopTo5 = Map.MapHeight;
                    for (j = 1; j <= loopTo5; j++)
                    {
                        xx = (short)(32 * (i - 1));
                        yy = (short)(32 * (j - 1));
                        ret = (short)BitBlt(withBlock.picMaskedBack.hDC, xx, yy, 32, 32, withBlock.picMask.hDC, 0, 0, SRCAND);
                        ret = (short)BitBlt(withBlock.picMaskedBack.hDC, xx, yy, 32, 32, withBlock.picMask2.hDC, 0, 0, SRCINVERT);
                    }
                }
            }

            // 画面を更新
            if (!string.IsNullOrEmpty(Map.MapFileName) & string.IsNullOrEmpty(draw_option))
            {
                RefreshScreen();
            }

            return;
            ErrorHandler:
            ;
            string argmsg = "マップ用ビットマップファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。";
            ErrorMessage(ref argmsg);
            SRC.TerminateSRC();
        }

        // 画面の書き換え (ユニット表示からやり直し)
        public static void RedrawScreen(bool late_refresh = false)
        {
            var PT = default(POINTAPI);
            int ret;
            ScreenIsMasked = false;

            // 画面を更新
            RefreshScreen(false, late_refresh);

            // カーソルを再描画
            GetCursorPos(ref PT);
            ret = SetCursorPos(PT.X, PT.Y);
        }

        // 画面をマスクがけして再表示
        public static void MaskScreen()
        {
            ScreenIsMasked = true;

            // 画面を更新
            RefreshScreen();
        }

        // ADD START MARGE
        // 画面の書き換え
        public static void RefreshScreen(bool without_refresh = false, bool delay_refresh = false)
        {
            if (NewGUIMode)
            {
                RefreshScreenNew(without_refresh, delay_refresh);
            }
            else
            {
                RefreshScreenOld(without_refresh, delay_refresh);
            }
        }
        // ADD END MARGE


        // 画面の書き換え (旧GUI)
        // MOD START MARGE
        // Public Sub RefreshScreen(Optional ByVal without_refresh As Boolean, _
        // '    Optional ByVal delay_refresh As Boolean)
        private static void RefreshScreenOld(bool without_refresh = false, bool delay_refresh = false)
        {
            // MOD END MARGE
            PictureBox pic;
            // UPGRADE_NOTE: my は my_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            short mx, my_Renamed;
            short sx, sy;
            short dx, dy;
            short dw, dh;
            short xx, yy;
            int ret;
            short i, j;
            Unit u;
            var PT = default(POINTAPI);
            int prev_color;

            // マップデータが設定されていなければ画面書き換えを行わない
            if (Map.MapWidth == 1)
            {
                return;
            }

            // 表示位置がマップ外にある場合はマップ内に合わせる
            if (MapX < 1)
            {
                MapX = 1;
            }
            else if (MapX > Map.MapWidth)
            {
                MapX = Map.MapWidth;
            }

            if (MapY < 1)
            {
                MapY = 1;
            }
            else if (MapY > Map.MapHeight)
            {
                MapY = Map.MapHeight;
            }

            {
                var withBlock = MainForm;
                pic = withBlock.picMain(0);
                if (!without_refresh)
                {
                    IsPictureVisible = false;
                    IsCursorVisible = false;
                    PaintedAreaX1 = MainPWidth;
                    PaintedAreaY1 = MainPHeight;
                    PaintedAreaX2 = -1;
                    PaintedAreaY2 = -1;

                    // マップウィンドウのスクロールバーの位置を変更
                    if (!IsGUILocked)
                    {
                        if (withBlock.HScroll.Value != MapX)
                        {
                            withBlock.HScroll.Value = MapX;
                            return;
                        }
                        if (withBlock.VScroll.Value != MapY)
                        {
                            withBlock.VScroll.Value = MapY;
                            return;
                        }
                    }

                    // 一旦マップウィンドウの内容を消去
                    ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS);
                }

                mx = (short)(MapX - (MainWidth + 1) / 2 + 1);
                my_Renamed = (short)(MapY - (MainHeight + 1) / 2 + 1);

                // マップ画像の転送元と転送先を計算する

                if (mx < 1)
                {
                    sx = 1;
                    dx = (short)(2 - mx);
                    dw = (short)(MainWidth - (1 - mx));
                }
                else if (mx + MainWidth - 1 > Map.MapWidth)
                {
                    sx = mx;
                    dx = 1;
                    dw = (short)(MainWidth - (mx + MainWidth - 1 - Map.MapWidth));
                }
                else
                {
                    sx = mx;
                    dx = 1;
                    dw = MainWidth;
                }

                if (dw > MainWidth)
                {
                    dw = MainWidth;
                }

                if (my_Renamed < 1)
                {
                    sy = 1;
                    dy = (short)(2 - my_Renamed);
                    dh = (short)(MainHeight - (1 - my_Renamed));
                }
                else if (my_Renamed + MainHeight - 1 > Map.MapHeight)
                {
                    sy = my_Renamed;
                    dy = 1;
                    dh = (short)(MainHeight - (my_Renamed + MainHeight - 1 - Map.MapHeight));
                }
                else
                {
                    sy = my_Renamed;
                    dy = 1;
                    dh = MainHeight;
                }

                if (dh > MainHeight)
                {
                    dh = MainHeight;
                }

                // 直線を描画する際の描画色を黒に変更
                prev_color = ColorTranslator.ToOle(pic.ForeColor);
                pic.ForeColor = Color.Black;

                // 表示内容を更新
                if (!ScreenIsMasked)
                {
                    // 通常表示
                    var loopTo = (short)(dw - 1);
                    for (i = 0; i <= loopTo; i++)
                    {
                        xx = (short)(32 * (dx + i - 1));
                        var loopTo1 = (short)(dh - 1);
                        for (j = 0; j <= loopTo1; j++)
                        {
                            if (sx + i < 1 | (short)(sx + i) > Map.MapWidth | sy + j < 1 | (short)(sy + j) > Map.MapHeight)
                            {
                                goto NextLoop;
                            }

                            yy = (short)(32 * (dy + j - 1));
                            u = Map.MapDataForUnit[(short)(sx + i), (short)(sy + j)];
                            if (u is null)
                            {
                                // 地形
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                            }
                            else if (u.BitmapID == -1)
                            {
                                // 非表示のユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                            }
                            else
                            {
                                string argfname = "地形ユニット";
                                if (u.Action > 0 | u.IsFeatureAvailable(ref argfname))
                                {
                                    // ユニット
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);
                                }
                                else
                                {
                                    // 行動済のユニット
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15) + 32, SRCCOPY);
                                }

                                // ユニットのいる場所に合わせて表示を変更
                                switch (u.Area ?? "")
                                {
                                    case "空中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            break;
                                        }

                                    case "水中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                                            break;
                                        }

                                    case "地中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                            {
                                                ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                                ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            }

                                            break;
                                        }
                                }
                            }

                            NextLoop:
                            ;
                        }
                    }
                }
                else
                {
                    // マスク表示
                    var loopTo2 = (short)(dw - 1);
                    for (i = 0; i <= loopTo2; i++)
                    {
                        xx = (short)(32 * (dx + i - 1));
                        var loopTo3 = (short)(dh - 1);
                        for (j = 0; j <= loopTo3; j++)
                        {
                            if (sx + i < 1 | (short)(sx + i) > Map.MapWidth | sy + j < 1 | (short)(sy + j) > Map.MapHeight)
                            {
                                goto NextLoop2;
                            }

                            yy = (short)(32 * (dy + j - 1));
                            u = Map.MapDataForUnit[(short)(sx + i), (short)(sy + j)];
                            if (u is null)
                            {
                                if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                                {
                                    // マスクされた地形
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                                }
                                else
                                {
                                    // 地形
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                                }
                            }
                            else if (u.BitmapID == -1)
                            {
                                // 非表示のユニット
                                if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                                {
                                    // マスクされた地形
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                                }
                                else
                                {
                                    // 地形
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                                }
                            }
                            else if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                            {
                                // マスクされたユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15) + 64, SRCCOPY);

                                // ユニットのいる場所に合わせて表示を変更
                                switch (u.Area ?? "")
                                {
                                    case "空中":
                                        {
                                            DottedLine(xx, (short)(yy + 28));
                                            break;
                                        }

                                    case "水中":
                                        {
                                            DottedLine(xx, (short)(yy + 3));
                                            break;
                                        }

                                    case "地中":
                                        {
                                            DottedLine(xx, (short)(yy + 28));
                                            DottedLine(xx, (short)(yy + 3));
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                            {
                                                DottedLine(xx, (short)(yy + 28));
                                            }

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                // ユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);

                                // ユニットのいる場所に合わせて表示を変更
                                switch (u.Area ?? "")
                                {
                                    case "空中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            break;
                                        }

                                    case "水中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                                            break;
                                        }

                                    case "地中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                            {
                                                ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                                ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            }

                                            break;
                                        }
                                }
                            }

                            NextLoop2:
                            ;
                        }
                    }
                }

                // 描画色を元に戻しておく
                pic.ForeColor = ColorTranslator.FromOle(prev_color);

                // 画面が書き換えられたことを記録
                ScreenIsSaved = false;
                if (!without_refresh & !delay_refresh)
                {
                    withBlock.picMain(0).Refresh();
                }
            }
        }

        // ADD START MARGE
        // 画面の書き換え (新GUI)
        private static void RefreshScreenNew(bool without_refresh = false, bool delay_refresh = false)
        {
            PictureBox pic;
            // UPGRADE_NOTE: my は my_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            short mx, my_Renamed;
            short sx, sy;
            short dx, dy;
            short dw, dh;
            short xx, yy;
            int ret;
            short i, j;
            Unit u;
            var PT = default(POINTAPI);
            int prev_color;

            // マップデータが設定されていなければ画面書き換えを行わない
            if (Map.MapWidth == 1)
            {
                return;
            }

            // 表示位置がマップ外にある場合はマップ内に合わせる
            if (MapX < 1)
            {
                MapX = 1;
            }
            else if (MapX > Map.MapWidth)
            {
                MapX = Map.MapWidth;
            }

            if (MapY < 1)
            {
                MapY = 1;
            }
            else if (MapY > Map.MapHeight)
            {
                MapY = Map.MapHeight;
            }

            {
                var withBlock = MainForm;
                pic = withBlock.picMain(0);
                if (!without_refresh)
                {
                    IsPictureVisible = false;
                    IsCursorVisible = false;
                    PaintedAreaX1 = MainPWidth;
                    PaintedAreaY1 = MainPHeight;
                    PaintedAreaX2 = -1;
                    PaintedAreaY2 = -1;

                    // マップウィンドウのスクロールバーの位置を変更
                    if (!IsGUILocked)
                    {
                        if (withBlock.HScroll.Value != MapX)
                        {
                            withBlock.HScroll.Value = MapX;
                            return;
                        }
                        if (withBlock.VScroll.Value != MapY)
                        {
                            withBlock.VScroll.Value = MapY;
                            return;
                        }
                    }

                    // 一旦マップウィンドウの内容を消去
                    ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS);
                }

                mx = (short)(MapX - (MainWidth + 1) / 2 + 1);
                my_Renamed = (short)(MapY - (MainHeight + 1) / 2 + 1);

                // マップ画像の転送元と転送先を計算する

                if (mx < 1)
                {
                    sx = 1;
                    dx = (short)(2 - mx);
                    dw = (short)(MainWidth - (1 - mx));
                }
                else if (mx + MainWidth - 1 > Map.MapWidth)
                {
                    sx = mx;
                    dx = 1;
                    dw = (short)(MainWidth - (mx + MainWidth - 1 - Map.MapWidth));
                }
                else
                {
                    sx = mx;
                    dx = 1;
                    dw = MainWidth;
                }

                if (dw > MainWidth)
                {
                    dw = MainWidth;
                }

                if (my_Renamed < 1)
                {
                    sy = 1;
                    dy = (short)(2 - my_Renamed);
                    dh = (short)(MainHeight - (1 - my_Renamed));
                }
                else if (my_Renamed + MainHeight - 1 > Map.MapHeight)
                {
                    sy = my_Renamed;
                    dy = 1;
                    dh = (short)(MainHeight - (my_Renamed + MainHeight - 1 - Map.MapHeight));
                }
                else
                {
                    sy = my_Renamed;
                    dy = 1;
                    dh = MainHeight;
                }

                if (dh > MainHeight)
                {
                    dh = MainHeight;
                }

                // 直線を描画する際の描画色を黒に変更
                prev_color = ColorTranslator.ToOle(pic.ForeColor);
                pic.ForeColor = Color.Black;

                // 表示内容を更新
                if (!ScreenIsMasked)
                {
                    // 通常表示
                    var loopTo = (short)(dw - 1);
                    for (i = -1; i <= loopTo; i++)
                    {
                        xx = (short)(32d * (dx + i - 0.5d));
                        var loopTo1 = (short)(dh - 1);
                        for (j = 0; j <= loopTo1; j++)
                        {
                            yy = (short)(32 * (dy + j - 1));
                            if (sx + i < 1 | (short)(sx + i) > Map.MapWidth | sy + j < 1 | (short)(sy + j) > Map.MapHeight)
                            {
                                goto NextLoop;
                            }

                            u = Map.MapDataForUnit[(short)(sx + i), (short)(sy + j)];
                            if (i == -1)
                            {
                                // 画面左端は16ピクセル幅分だけ表示
                                if (u is null)
                                {
                                    // 地形
                                    ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picBack.hDC, 32d * ((double)sx - 1.5d), 32 * (sy + j - 1), SRCCOPY);
                                }
                                else if (u.BitmapID == -1)
                                {
                                    // 非表示のユニット
                                    ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picBack.hDC, 32d * ((double)sx - 1.5d), 32 * (sy + j - 1), SRCCOPY);
                                }
                                else
                                {
                                    string argfname = "地形ユニット";
                                    if (u.Action > 0 | u.IsFeatureAvailable(ref argfname))
                                    {
                                        // ユニット
                                        ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15) + 16, 96 * ((int)u.BitmapID / 15), SRCCOPY);
                                    }
                                    else
                                    {
                                        // 行動済のユニット
                                        ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15) + 16, 96 * ((int)u.BitmapID / 15) + 32, SRCCOPY);
                                    }

                                    // ユニットのいる場所に合わせて表示を変更
                                    switch (u.Area ?? "")
                                    {
                                        case "空中":
                                            {
                                                ret = MoveToEx(pic.hDC, 0, (int)yy + 28, ref PT);
                                                ret = LineTo(pic.hDC, 0 + 15, (int)yy + 28);
                                                break;
                                            }

                                        case "水中":
                                            {
                                                ret = MoveToEx(pic.hDC, 0, (int)yy + 3, ref PT);
                                                ret = LineTo(pic.hDC, 0 + 15, (int)yy + 3);
                                                break;
                                            }

                                        case "地中":
                                            {
                                                ret = MoveToEx(pic.hDC, 0, (int)yy + 28, ref PT);
                                                ret = LineTo(pic.hDC, 0 + 15, (int)yy + 28);
                                                ret = MoveToEx(pic.hDC, 0, (int)yy + 3, ref PT);
                                                ret = LineTo(pic.hDC, 0 + 15, (int)yy + 3);
                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                                {
                                                    ret = MoveToEx(pic.hDC, 0, (int)yy + 28, ref PT);
                                                    ret = LineTo(pic.hDC, 0 + 15, (int)yy + 28);
                                                }

                                                break;
                                            }
                                    }
                                }
                            }
                            // 画面左端以外は全32ピクセル幅分だけ表示
                            else if (u is null)
                            {
                                // 地形
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                            }
                            else if (u.BitmapID == -1)
                            {
                                // 非表示のユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                            }
                            else
                            {
                                string argfname1 = "地形ユニット";
                                if (u.Action > 0 | u.IsFeatureAvailable(ref argfname1))
                                {
                                    // ユニット
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);
                                }
                                else
                                {
                                    // 行動済のユニット
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15) + 32, SRCCOPY);
                                }

                                // ユニットのいる場所に合わせて表示を変更
                                switch (u.Area ?? "")
                                {
                                    case "空中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            break;
                                        }

                                    case "水中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                                            break;
                                        }

                                    case "地中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                            {
                                                ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                                ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            }

                                            break;
                                        }
                                }
                            }

                            NextLoop:
                            ;
                        }
                    }
                }
                else
                {
                    // マスク表示
                    var loopTo2 = (short)(dw - 1);
                    for (i = -1; i <= loopTo2; i++)
                    {
                        xx = (short)(32d * (dx + i - 0.5d));
                        var loopTo3 = (short)(dh - 1);
                        for (j = 0; j <= loopTo3; j++)
                        {
                            yy = (short)(32 * (dy + j - 1));
                            if (sx + i < 1 | (short)(sx + i) > Map.MapWidth | sy + j < 1 | (short)(sy + j) > Map.MapHeight)
                            {
                                goto NextLoop2;
                            }

                            u = Map.MapDataForUnit[(short)(sx + i), (short)(sy + j)];
                            if (i == -1)
                            {
                                // 画面左端は16ピクセル幅分だけ表示
                                if (u is null)
                                {
                                    if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                                    {
                                        // マスクされた地形
                                        ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picMaskedBack.hDC, 32d * ((double)sx - 1.5d), 32 * (sy + j - 1), SRCCOPY);
                                    }
                                    else
                                    {
                                        // 地形
                                        ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picBack.hDC, 32d * ((double)sx - 1.5d), 32 * (sy + j - 1), SRCCOPY);
                                    }
                                }
                                else if (u.BitmapID == -1)
                                {
                                    // 非表示のユニット
                                    if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                                    {
                                        // マスクされた地形
                                        ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picMaskedBack.hDC, 32d * ((double)sx - 1.5d), 32 * (sy + j - 1), SRCCOPY);
                                    }
                                    else
                                    {
                                        // 地形
                                        ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picBack.hDC, 32d * ((double)sx - 1.5d), 32 * (sy + j - 1), SRCCOPY);
                                    }
                                }
                                else if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                                {
                                    // マスクされたユニット
                                    ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15) + 16, 96 * ((int)u.BitmapID / 15) + 64, SRCCOPY);

                                    // ユニットのいる場所に合わせて表示を変更
                                    switch (u.Area ?? "")
                                    {
                                        case "空中":
                                            {
                                                DottedLine(0, (short)(yy + 28), true);
                                                break;
                                            }

                                        case "水中":
                                            {
                                                DottedLine(0, (short)(yy + 3), true);
                                                break;
                                            }

                                        case "地中":
                                            {
                                                DottedLine(0, (short)(yy + 28), true);
                                                DottedLine(0, (short)(yy + 3), true);
                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                                {
                                                    DottedLine(0, (short)(yy + 28), true);
                                                }

                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    // ユニット
                                    ret = BitBlt(pic.hDC, 0, yy, 16, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15) + 16, 96 * ((int)u.BitmapID / 15), SRCCOPY);

                                    // ユニットのいる場所に合わせて表示を変更
                                    switch (u.Area ?? "")
                                    {
                                        case "空中":
                                            {
                                                ret = MoveToEx(pic.hDC, 0, (int)yy + 28, ref PT);
                                                ret = LineTo(pic.hDC, 0 + 15, (int)yy + 28);
                                                break;
                                            }

                                        case "水中":
                                            {
                                                ret = MoveToEx(pic.hDC, 0, (int)yy + 3, ref PT);
                                                ret = LineTo(pic.hDC, 0 + 15, (int)yy + 3);
                                                break;
                                            }

                                        case "地中":
                                            {
                                                ret = MoveToEx(pic.hDC, 0, (int)yy + 28, ref PT);
                                                ret = LineTo(pic.hDC, 0 + 15, (int)yy + 28);
                                                ret = MoveToEx(pic.hDC, 0, (int)yy + 3, ref PT);
                                                ret = LineTo(pic.hDC, 0 + 15, (int)yy + 3);
                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                                {
                                                    ret = MoveToEx(pic.hDC, 0, (int)yy + 28, ref PT);
                                                    ret = LineTo(pic.hDC, 0 + 15, (int)yy + 28);
                                                }

                                                break;
                                            }
                                    }
                                }
                            }
                            // 画面左端以外は全32ピクセル幅分だけ表示
                            else if (u is null)
                            {
                                if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                                {
                                    // マスクされた地形
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                                }
                                else
                                {
                                    // 地形
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                                }
                            }
                            else if (u.BitmapID == -1)
                            {
                                // 非表示のユニット
                                if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                                {
                                    // マスクされた地形
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                                }
                                else
                                {
                                    // 地形
                                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY);
                                }
                            }
                            else if (Map.MaskData[(short)(sx + i), (short)(sy + j)])
                            {
                                // マスクされたユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15) + 64, SRCCOPY);

                                // ユニットのいる場所に合わせて表示を変更
                                switch (u.Area ?? "")
                                {
                                    case "空中":
                                        {
                                            DottedLine(xx, (short)(yy + 28));
                                            break;
                                        }

                                    case "水中":
                                        {
                                            DottedLine(xx, (short)(yy + 3));
                                            break;
                                        }

                                    case "地中":
                                        {
                                            DottedLine(xx, (short)(yy + 28));
                                            DottedLine(xx, (short)(yy + 3));
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                            {
                                                DottedLine(xx, (short)(yy + 28));
                                            }

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                // ユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);

                                // ユニットのいる場所に合わせて表示を変更
                                switch (u.Area ?? "")
                                {
                                    case "空中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            break;
                                        }

                                    case "水中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                                            break;
                                        }

                                    case "地中":
                                        {
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (Map.TerrainClass((short)(sx + i), (short)(sy + j)) == "月面")
                                            {
                                                ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                                ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                                            }

                                            break;
                                        }
                                }
                            }

                            NextLoop2:
                            ;
                        }
                    }
                }

                // 描画色を元に戻しておく
                pic.ForeColor = ColorTranslator.FromOle(prev_color);

                // 画面が書き換えられたことを記録
                ScreenIsSaved = false;
                if (!without_refresh & !delay_refresh)
                {
                    withBlock.picMain(0).Refresh();
                }
            }
        }
        // ADD END MARGE

        // MOD START MARGE
        // Private Sub DottedLine(ByVal X As Integer, ByVal Y As Integer)
        private static void DottedLine(short X, short Y, bool half_size = false)
        {
            // MOD END MARGE
            short i;

            {
                var withBlock = MainForm.picMain(0);
                // MOD START MARGE
                // For i = 0 To 15
                // MainForm.picMain(0).PSet (X + 2 * i + 1, Y), vbBlack
                // Next
                if (half_size)
                {
                    for (i = 0; i <= 7; i++)
                        MainForm.picMain(0).PSet(new Point(), Y);/* TODO ERROR: Skipped SkippedTokensTrivia */
                }
                else
                {
                    for (i = 0; i <= 15; i++)
                        MainForm.picMain(0).PSet(new Point(), Y);/* TODO ERROR: Skipped SkippedTokensTrivia */
                }
                // MOD END MARGE
            }
        }

        // 指定されたマップ座標を画面の中央に表示
        public static void Center(short new_x, short new_y)
        {
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                new_x = (short)((MainWidth + 1) / 2);
                if (new_y < (Map.MapHeight + 1) / 2)
                {
                    new_y = (short)((Map.MapHeight + 1) / 2);
                }
                else if (new_y > Map.MapHeight - (MainWidth + 1) / 2 + 1)
                {
                    new_y = (short)(Map.MapHeight - (MainWidth + 1) / 2 + 1);
                }

                return;
            }

            MapX = new_x;
            if (MapX < 1)
            {
                MapX = 1;
            }
            else if (MapX > MainForm.HScroll.max)
            {
                MapX = MainForm.HScroll.max;
            }

            MapY = new_y;
            if (MapY < 1)
            {
                MapY = 1;
            }
            else if (MapY > MainForm.VScroll.max)
            {
                MapY = MainForm.VScroll.max;
            }
        }


        // === 座標変換 ===

        // マップ上での座標がマップ画面のどの位置にくるかを返す
        public static short MapToPixelX(short X)
        {
            short MapToPixelXRet = default;
            // MOD START MARGE
            // MapToPixelX = 32 * ((MainWidth + 1) \ 2 - 1 - (MapX - X))
            if (NewGUIMode)
            {
                MapToPixelXRet = (short)(32d * ((MainWidth + 1) / 2 - 0.5d - (MapX - X)));
            }
            else
            {
                MapToPixelXRet = (short)(32 * ((MainWidth + 1) / 2 - 1 - (MapX - X)));
            }

            return MapToPixelXRet;
            // MOD END MARGE
        }

        public static short MapToPixelY(short Y)
        {
            short MapToPixelYRet = default;
            MapToPixelYRet = (short)(32 * ((MainHeight + 1) / 2 - 1 - (MapY - Y)));
            return MapToPixelYRet;
        }

        // マップ画面でのピクセルがマップの座標のどの位置にくるかを返す
        public static short PixelToMapX(short X)
        {
            short PixelToMapXRet = default;
            if (X < 0)
            {
                X = 0;
            }
            else if (X >= MainPWidth)
            {
                X = (short)(MainPWidth - 1);
            }

            // MOD START MARGE
            // PixelToMapX = X \ 32 + 1 + MapX - (MainWidth + 1) \ 2
            if (NewGUIMode)
            {
                PixelToMapXRet = (short)((X + 16) / 32 + MapX - (MainWidth + 1) / 2);
            }
            else
            {
                PixelToMapXRet = (short)(X / 32 + 1 + MapX - (MainWidth + 1) / 2);
            }

            return PixelToMapXRet;
            // MOD END MARGE
        }

        public static short PixelToMapY(short Y)
        {
            short PixelToMapYRet = default;
            if (Y < 0)
            {
                Y = 0;
            }
            else if (Y >= MainPHeight)
            {
                Y = (short)(MainPHeight - 1);
            }

            PixelToMapYRet = (short)(Y / 32 + 1 + MapY - (MainHeight + 1) / 2);
            return PixelToMapYRet;
        }


        // === ユニット画像表示に関する処理 ===

        // ユニット画像ファイルを検索
        private static string FindUnitBitmap(ref Unit u)
        {
            string FindUnitBitmapRet = default;
            string fname, uname;
            string tnum, tname = default, tdir = default;
            short i, j;
            // インターミッションでのパイロットステータス表示の場合は
            // 特殊な処理が必要
            string argfname = "ダミーユニット";
            if (u.IsFeatureAvailable(ref argfname) & Strings.InStr(u.Name, "ステータス表示用ユニット") == 0)
            {
                if (u.CountPilot() == 0)
                {
                    return FindUnitBitmapRet;
                }

                object argIndex1 = "ダミーユニット";
                if (u.FeatureData(ref argIndex1) == "ユニット画像使用")
                {
                    // ユニット画像を使って表示
                    uname = "搭乗ユニット[" + u.MainPilot().ID + "]";
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    uname = Conversions.ToString(Event_Renamed.LocalVariableList[uname].StringValue);
                    Unit localItem() { object argIndex1 = uname; var ret = SRC.UList.Item(ref argIndex1); return ret; }

                    fname = @"\Bitmap\Unit\" + localItem().get_Bitmap(false);
                }
                else
                {
                    // パイロット画像を使って表示
                    fname = @"\Bitmap\Pilot\" + u.MainPilot().get_Bitmap(false);
                }

                // 画像を検索
                bool localFileExists() { string argfname = SRC.ScenarioPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists1() { string argfname = SRC.ExtDataPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists2() { string argfname = SRC.ExtDataPath2 + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists3() { string argfname = SRC.AppPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                if (Strings.InStr(fname, @"\-.bmp") > 0)
                {
                    fname = "";
                }
                else if (localFileExists())
                {
                    fname = SRC.ScenarioPath + fname;
                }
                else if (localFileExists1())
                {
                    fname = SRC.ExtDataPath + fname;
                }
                else if (localFileExists2())
                {
                    fname = SRC.ExtDataPath2 + fname;
                }
                else if (localFileExists3())
                {
                    fname = SRC.AppPath + fname;
                }
                else
                {
                    fname = "";
                }

                FindUnitBitmapRet = fname;
                return FindUnitBitmapRet;
            }

            string argfname3 = "地形ユニット";
            if (u.IsFeatureAvailable(ref argfname3))
            {
                // 地形ユニット
                fname = u.get_Bitmap(false);
                bool localFileExists5() { string argfname = SRC.ScenarioPath + @"Bitmap\Map\" + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                string argfname2 = SRC.AppPath + @"Bitmap\Map\" + fname;
                if (GeneralLib.FileExists(ref argfname2) | localFileExists5())
                {
                    fname = @"Bitmap\Map\" + fname;
                }
                else
                {
                    // 地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
                    i = (short)(Strings.Len(fname) - 5);
                    while (i > 0)
                    {
                        if (LikeOperator.LikeString(Strings.Mid(fname, i, 1), "[!-0-9]", CompareMethod.Binary))
                        {
                            break;
                        }

                        i = (short)(i - 1);
                    }

                    if (i > 0)
                    {
                        tdir = Strings.Left(fname, i);
                        {
                            var withBlock = SRC.TDList;
                            var loopTo = withBlock.Count;
                            for (j = 1; j <= loopTo; j++)
                            {
                                if (tdir == withBlock.Item(withBlock.OrderedID(j)).Bitmap)
                                {
                                    tnum = Strings.Mid(fname, i + 1, Strings.Len(fname) - i - 4);
                                    tname = Strings.Left(fname, i) + SrcFormatter.Format(GeneralLib.StrToLng(ref tnum), "0000") + ".bmp";
                                    break;
                                }
                            }

                            if (j <= withBlock.Count)
                            {
                                tdir = tdir + @"\";
                            }
                            else
                            {
                                tdir = "";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(tdir))
                    {
                        bool localFileExists4() { string argfname = SRC.ScenarioPath + @"Bitmap\Map\" + tname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                        string argfname1 = SRC.AppPath + @"Bitmap\Map\" + tname;
                        if (GeneralLib.FileExists(ref argfname1) | localFileExists4())
                        {
                            fname = @"Bitmap\Map\" + tname;
                        }
                        else
                        {
                            fname = @"Bitmap\Map\" + tdir + @"\" + tname;
                        }
                    }
                    else if (Strings.InStr(fname, @"\") > 0)
                    {
                        // フォルダ指定あり
                        fname = @"Bitmap\" + fname;
                    }
                    else
                    {
                        // 通常のユニット画像
                        fname = @"Bitmap\Unit\" + fname;
                    }
                }
            }
            else
            {
                // 通常のユニット描画
                fname = u.get_Bitmap(false);
                if (Strings.InStr(fname, ":") == 2)
                {
                }
                // フルパス指定
                else if (Strings.InStr(fname, @"\") > 0)
                {
                    // フォルダ指定あり
                    fname = @"Bitmap\" + fname;
                }
                else
                {
                    // 通常のユニット画像
                    fname = @"Bitmap\Unit\" + fname;
                }
            }

            // 画像の検索
            bool localFileExists6() { string argfname = SRC.ScenarioPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            bool localFileExists7() { string argfname = SRC.ExtDataPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            bool localFileExists8() { string argfname = SRC.ExtDataPath2 + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            bool localFileExists9() { string argfname = SRC.AppPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            if (Strings.InStr(fname, @"\-.bmp") > 0)
            {
                fname = "";
            }
            else if (localFileExists6())
            {
                fname = SRC.ScenarioPath + fname;
            }
            else if (localFileExists7())
            {
                fname = SRC.ExtDataPath + fname;
            }
            else if (localFileExists8())
            {
                fname = SRC.ExtDataPath2 + fname;
            }
            else if (localFileExists9())
            {
                fname = SRC.AppPath + fname;
            }
            else if (!GeneralLib.FileExists(ref fname))
            {
                fname = "";

                // 画像が見つからなかったことを記録
                if ((u.get_Bitmap(false) ?? "") == (u.Data.Bitmap ?? ""))
                {
                    u.Data.IsBitmapMissing = true;
                }
            }

            FindUnitBitmapRet = fname;
            return FindUnitBitmapRet;
        }

        // ユニットのビットマップを作成
        public static short MakeUnitBitmap(ref Unit u)
        {
            short MakeUnitBitmapRet = default;
            string fname, uparty;
            short i;
            int ret;
            short xx, yy;
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
                    Static bitmap_num As Short

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
                    Static fname_list() As String

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
                    Static party_list() As String

             */
            {
                var withBlock = MainForm;
                string argfname = "非表示";
                if (u.IsFeatureAvailable(ref argfname))
                {
                    MakeUnitBitmapRet = -1;
                    return MakeUnitBitmapRet;
                }

                // 画像がクリアされている？
                if (withBlock.picUnitBitmap.width == 32)
                {
                    bitmap_num = 0;
                }

                // 今までにロードされているユニット画像数
                Array.Resize(ref fname_list, (int)(bitmap_num + 1));
                Array.Resize(ref party_list, (int)(bitmap_num + 1));

                // 以前ロードしたユニット画像と一致している？
                fname = FindUnitBitmap(ref u);
                uparty = u.Party0;
                var loopTo = bitmap_num;
                for (i = 1; i <= loopTo; i++)
                {
                    if ((fname ?? "") == (fname_list[i] ?? "") & (uparty ?? "") == (party_list[i] ?? ""))
                    {
                        // 一致したものが見つかった
                        MakeUnitBitmapRet = i;
                        return MakeUnitBitmapRet;
                    }
                }

                // 新たに画像を登録
                bitmap_num = (short)((int)bitmap_num + 1);
                Array.Resize(ref fname_list, (int)(bitmap_num + 1));
                Array.Resize(ref party_list, (int)(bitmap_num + 1));
                fname_list[(int)bitmap_num] = fname;
                party_list[(int)bitmap_num] = uparty;

                // 画像バッファの大きさを変更
                withBlock.picUnitBitmap.Move(0, 0, 480, 96 * ((int)bitmap_num / 15 + 1));

                // 画像の書き込み位置
                xx = (short)(32 * ((int)bitmap_num % 15));
                yy = (short)(96 * ((int)bitmap_num / 15));

                // ファイルをロードする
                LoadUnitBitmap(ref u, ref withBlock.picUnitBitmap, xx, yy, false, ref fname);

                // 行動済みの際の画像を作成
                ret = BitBlt(withBlock.picUnitBitmap.hDC, xx, (int)yy + 32, 32, 32, withBlock.picUnitBitmap.hDC, xx, yy, SRCCOPY);
                ret = BitBlt(withBlock.picUnitBitmap.hDC, xx, (int)yy + 32, 32, 32, withBlock.picMask.hDC, 0, 0, SRCAND);

                // マスク入りの画像を作成
                ret = BitBlt(withBlock.picUnitBitmap.hDC, xx, (int)yy + 64, 32, 32, withBlock.picUnitBitmap.hDC, xx, (int)yy + 32, SRCCOPY);
                ret = BitBlt(withBlock.picUnitBitmap.hDC, xx, (int)yy + 64, 32, 32, withBlock.picMask2.hDC, 0, 0, SRCINVERT);
            }

            // ユニット画像番号を返す
            MakeUnitBitmapRet = bitmap_num;
            return MakeUnitBitmapRet;
        }

        // ユニットのビットマップをロード
        public static void LoadUnitBitmap(ref Unit u, ref PictureBox pic, short dx, short dy, bool use_orig_color = false, [Optional, DefaultParameterValue("")] ref string fname)
        {
            int ret;
            var emit_light = default(bool);
            {
                var withBlock = MainForm;
                // 画像ファイルを検索
                if (string.IsNullOrEmpty(fname))
                {
                    fname = FindUnitBitmap(ref u);
                }

                // 画像をそのまま使用する場合
                // 画像の読み込み
                object argIndex1 = "ダミーユニット";
                if (Strings.InStr(fname, @"\Pilot\") > 0 | u.FeatureData(ref argIndex1) == "ユニット画像使用")
                {
                    ;
                    withBlock.picTmp = Image.FromFile(fname);
                    ;

                    // 画面に描画
                    ret = StretchBlt(pic.hDC, dx, dy, 32, 32, withBlock.picTmp.hDC, 0, 0, withBlock.picTmp.width, withBlock.picTmp.Height, SRCCOPY);
                    return;
                }

                // ユニットが自分で発光しているかをあらかじめチェック
                string argfname = "発光";
                if (Map.MapDrawMode == "夜" & !Map.MapDrawIsMapOnly & !use_orig_color & u.IsFeatureAvailable(ref argfname))
                {
                    emit_light = true;
                }
                // 画像が見つかった場合は画像を読み込み
                if (!string.IsNullOrEmpty(fname))
                {
                    ;
                    withBlock.picTmp32(0) = Image.FromFile(fname);
                    ;

                    // 画像のサイズが正しいかチェック
                    if (withBlock.picTmp32(0).width != 32 | withBlock.picTmp32(0).Height != 32)
                    {
                        {
                            var withBlock1 = withBlock.picTmp32(0);
                            withBlock1.Picture = Image.FromFile("");
                            withBlock1.width = 32;
                            withBlock1.Height = 32;
                        }

                        string argmsg = u.Name + "のユニット画像が32x32の大きさになっていません";
                        ErrorMessage(ref argmsg);
                        return;
                    }

                    string argfname1 = "地形ユニット";
                    if (u.IsFeatureAvailable(ref argfname1))
                    {
                        // 地形ユニットの場合は画像をそのまま使う
                        ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, SRCCOPY);
                    }
                    else if (SRC.UseTransparentBlt)
                    {
                        // TransparentBltを使ってユニット画像とタイルを重ね合わせる

                        // タイル
                        switch (u.Party0 ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                                {
                                    ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picUnit.hDC, 0, 0, SRCCOPY);
                                    break;
                                }

                            case "敵":
                                {
                                    ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picEnemy.hDC, 0, 0, SRCCOPY);
                                    break;
                                }

                            case "中立":
                                {
                                    ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picNeautral.hDC, 0, 0, SRCCOPY);
                                    break;
                                }
                        }

                        // 画像の重ね合わせ
                        // (発光している場合は２度塗りを防ぐため描画しない)
                        if (!emit_light)
                        {
                            ret = TransparentBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, 32, 32, ColorTranslator.ToOle(Color.White));
                        }
                    }
                    else
                    {
                        // BitBltを使ってユニット画像とタイルを重ね合わせる

                        // マスクを作成
                        int argw = 32;
                        int argh = 32;
                        int argtcolor = ColorTranslator.ToOle(Color.White);
                        Graphics.MakeMask(ref withBlock.picTmp32(0).hDC, ref withBlock.picTmp32(2).hDC, ref argw, ref argh, ref argtcolor);

                        // タイル
                        switch (u.Party0 ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                                {
                                    ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picUnit.hDC, 0, 0, SRCCOPY);
                                    break;
                                }

                            case "敵":
                                {
                                    ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picEnemy.hDC, 0, 0, SRCCOPY);
                                    break;
                                }

                            case "中立":
                                {
                                    ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picNeautral.hDC, 0, 0, SRCCOPY);
                                    break;
                                }
                        }

                        // 画像の重ね合わせ
                        // (発光している場合は２度塗りを防ぐため描画しない)
                        if (!emit_light)
                        {
                            ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(2).hDC, 0, 0, SRCERASE);
                            ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, SRCINVERT);
                        }
                    }
                }
                else
                {
                    // 画像が見つからなかった場合はタイルのみでユニット画像を作成
                    switch (u.Party0 ?? "")
                    {
                        case "味方":
                        case "ＮＰＣ":
                            {
                                ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picUnit.hDC, 0, 0, SRCCOPY);
                                break;
                            }

                        case "敵":
                            {
                                ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picEnemy.hDC, 0, 0, SRCCOPY);
                                break;
                            }

                        case "中立":
                            {
                                ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picNeautral.hDC, 0, 0, SRCCOPY);
                                break;
                            }
                    }
                }

                // 色をステージの状況に合わせて変更
                if (!use_orig_color & !Map.MapDrawIsMapOnly)
                {
                    switch (Map.MapDrawMode ?? "")
                    {
                        case "夜":
                            {
                                var argpic = withBlock.picTmp32(1);
                                Graphics.GetImage(ref argpic);
                                Graphics.Dark();
                                var argpic1 = withBlock.picTmp32(1);
                                Graphics.SetImage(ref argpic1);
                                // ユニットが"発光"の特殊能力を持つ場合、
                                // ユニット画像を、暗くしたタイル画像の上に描画する。
                                if (emit_light)
                                {
                                    if (SRC.UseTransparentBlt)
                                    {
                                        ret = TransparentBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, 32, 32, ColorTranslator.ToOle(Color.White));
                                    }
                                    else
                                    {
                                        ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(2).hDC, 0, 0, SRCERASE);
                                        ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, SRCINVERT);
                                    }
                                }

                                break;
                            }

                        case "セピア":
                            {
                                var argpic2 = withBlock.picTmp32(1);
                                Graphics.GetImage(ref argpic2);
                                Graphics.Sepia();
                                var argpic3 = withBlock.picTmp32(1);
                                Graphics.SetImage(ref argpic3);
                                break;
                            }

                        case "白黒":
                            {
                                var argpic4 = withBlock.picTmp32(1);
                                Graphics.GetImage(ref argpic4);
                                Graphics.Monotone();
                                var argpic5 = withBlock.picTmp32(1);
                                Graphics.SetImage(ref argpic5);
                                break;
                            }

                        case "夕焼け":
                            {
                                var argpic6 = withBlock.picTmp32(1);
                                Graphics.GetImage(ref argpic6);
                                Graphics.Sunset();
                                var argpic7 = withBlock.picTmp32(1);
                                Graphics.SetImage(ref argpic7);
                                break;
                            }

                        case "水中":
                            {
                                var argpic8 = withBlock.picTmp32(1);
                                Graphics.GetImage(ref argpic8);
                                Graphics.Water();
                                var argpic9 = withBlock.picTmp32(1);
                                Graphics.SetImage(ref argpic9);
                                break;
                            }

                        case "フィルタ":
                            {
                                var argpic10 = withBlock.picTmp32(1);
                                Graphics.GetImage(ref argpic10);
                                Graphics.ColorFilter(ref Map.MapDrawFilterColor, ref Map.MapDrawFilterTransPercent);
                                var argpic11 = withBlock.picTmp32(1);
                                Graphics.SetImage(ref argpic11);
                                break;
                            }
                    }
                }

                // 画面に描画
                ret = BitBlt(pic.hDC, dx, dy, 32, 32, withBlock.picTmp32(1).hDC, 0, 0, SRCCOPY);
            }

            return;
            ErrorHandler:
            ;
            string argmsg1 = "ユニット用ビットマップファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。";
            ErrorMessage(ref argmsg1);
        }

        // ユニット画像の描画
        public static void PaintUnitBitmap(ref Unit u, string smode = "")
        {
            short xx, yy;
            PictureBox pic;
            int ret;
            var PT = default(POINTAPI);
            // 非表示？
            if (u.BitmapID == -1)
            {
                return;
            }

            // 画面外？
            if (u.x < MapX - (MainWidth + 1) / 2 | MapX + (MainWidth + 1) / 2 < u.x | u.y < MapY - (MainHeight + 1) / 2 | MapY + (MainHeight + 1) / 2 < u.y)
            {
                return;
            }

            // 描き込み先の座標を設定
            xx = MapToPixelX(u.x);
            yy = MapToPixelY(u.y);
            {
                var withBlock = MainForm;
                if (smode == "リフレッシュ無し" & ScreenIsSaved)
                {
                    pic = withBlock.picMain(1);
                    // 表示画像を消去する際に使う描画領域を設定
                    PaintedAreaX1 = (short)GeneralLib.MinLng(PaintedAreaX1, GeneralLib.MaxLng(xx, 0));
                    PaintedAreaY1 = (short)GeneralLib.MinLng(PaintedAreaY1, GeneralLib.MaxLng(yy, 0));
                    PaintedAreaX2 = (short)GeneralLib.MaxLng(PaintedAreaX2, GeneralLib.MinLng(xx + 32, MainPWidth - 1));
                    PaintedAreaY2 = (short)GeneralLib.MaxLng(PaintedAreaY2, GeneralLib.MinLng(yy + 32, MainPHeight - 1));
                }
                else
                {
                    pic = withBlock.picMain(0);
                }

                // ユニット画像の書き込み
                string argfname = "地形ユニット";
                if (u.Action > 0 | u.IsFeatureAvailable(ref argfname))
                {
                    // 通常の表示
                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);
                }
                else
                {
                    // 行動済の場合は網掛け
                    ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15) + 32, SRCCOPY);
                }

                // 直線を描画する際の描画色を設定
                pic.ForeColor = Color.Black;

                // ユニットのいる場所に合わせて表示を変更
                switch (u.Area ?? "")
                {
                    case "空中":
                        {
                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                            break;
                        }

                    case "水中":
                        {
                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                            break;
                        }

                    case "地中":
                        {
                            ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                            ret = MoveToEx(pic.hDC, xx, (int)yy + 3, ref PT);
                            ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 3);
                            break;
                        }

                    case "宇宙":
                        {
                            if (Map.TerrainClass(u.x, u.y) == "月面")
                            {
                                ret = MoveToEx(pic.hDC, xx, (int)yy + 28, ref PT);
                                ret = LineTo(pic.hDC, (int)xx + 31, (int)yy + 28);
                            }

                            break;
                        }
                }

                // 描画色を白に戻しておく
                pic.ForeColor = Color.White;
                if (smode != "リフレッシュ無し")
                {
                    // 画面が書き換えられたことを記録
                    ScreenIsSaved = false;
                    if (withBlock.Visible)
                    {
                        pic.Refresh();
                    }
                }
            }
        }

        // ユニット画像の表示を消す
        public static void EraseUnitBitmap(short X, short Y, bool do_refresh = true)
        {
            short xx, yy;
            int ret;

            // 画面外？
            if (X < MapX - (MainWidth + 1) / 2 | MapX + (MainWidth + 1) / 2 < X | Y < MapY - (MainHeight + 1) / 2 | MapY + (MainHeight + 1) / 2 < Y)
            {
                return;
            }

            // 画面が乱れるので書き換えない？
            if (IsPictureVisible)
            {
                return;
            }

            xx = MapToPixelX(X);
            yy = MapToPixelY(Y);
            {
                var withBlock = MainForm;
                SaveScreen();

                // 画面表示変更
                ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * ((int)X - 1), 32 * ((int)Y - 1), SRCCOPY);
                ret = BitBlt(withBlock.picMain(1).hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * ((int)X - 1), 32 * ((int)Y - 1), SRCCOPY);
                if (do_refresh)
                {
                    // 画面が書き換えられたことを記録
                    ScreenIsSaved = false;
                    if (withBlock.Visible)
                    {
                        withBlock.picMain(0).Refresh();
                    }
                }
            }
        }

        // ユニット画像の表示位置を移動 (アニメーション)
        public static void MoveUnitBitmap(ref Unit u, short x1, short y1, short x2, short y2, int wait_time0, short division = 2)
        {
            short xx, yy;
            short vx, vy;
            int ret;
            short i;
            PictureBox pic;
            int cur_time, start_time = default, wait_time;
            var PT = default(POINTAPI);
            wait_time = wait_time0 / division;
            SaveScreen();
            {
                var withBlock = MainForm;
                pic = withBlock.picTmp32(0);

                // ユニット画像を作成
                ret = BitBlt(pic.hDC, 0, 0, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);

                // ユニットのいる場所に合わせて表示を変更
                switch (u.Area ?? "")
                {
                    case "空中":
                        {
                            ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                            ret = LineTo(pic.hDC, 31, 28);
                            break;
                        }

                    case "水中":
                        {
                            ret = MoveToEx(pic.hDC, 0, 3, ref PT);
                            ret = LineTo(pic.hDC, 31, 3);
                            break;
                        }

                    case "地中":
                        {
                            ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                            ret = LineTo(pic.hDC, 31, 28);
                            ret = MoveToEx(pic.hDC, 0, 3, ref PT);
                            ret = LineTo(pic.hDC, 31, 3);
                            break;
                        }

                    case "宇宙":
                        {
                            if (Map.TerrainClass(u.x, u.y) == "月面")
                            {
                                ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                                ret = LineTo(pic.hDC, 31, 28);
                            }

                            break;
                        }
                }

                // 移動の始点を設定
                xx = MapToPixelX(x1);
                yy = MapToPixelY(y1);

                // 背景上の画像をまず消去
                // (既に移動している場合を除く)
                if (ReferenceEquals(u, Map.MapDataForUnit[x1, y1]))
                {
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * ((int)x1 - 1), 32 * ((int)y1 - 1), SRCCOPY);
                    ret = BitBlt(withBlock.picMain(1).hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * ((int)x1 - 1), 32 * ((int)y1 - 1), SRCCOPY);
                }

                // 最初の移動方向を設定
                if (Math.Abs((short)(x2 - x1)) > Math.Abs((short)(y2 - y1)))
                {
                    if (x2 > x1)
                    {
                        vx = 1;
                    }
                    else
                    {
                        vx = -1;
                    }

                    vy = 0;
                }
                else
                {
                    if (y2 > y1)
                    {
                        vy = 1;
                    }
                    else
                    {
                        vy = -1;
                    }

                    vx = 0;
                }

                if (wait_time > 0)
                {
                    start_time = GeneralLib.timeGetTime();
                }

                // 移動の描画
                var loopTo = (short)(division * GeneralLib.MaxLng(Math.Abs((short)(x2 - x1)), Math.Abs((short)(y2 - y1))));
                for (i = 1; i <= loopTo; i++)
                {
                    // 画像を消去
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, withBlock.picMain(1).hDC, xx, yy, SRCCOPY);

                    // 座標を移動
                    xx = (short)(xx + 32 * vx / division);
                    yy = (short)(yy + 32 * vy / division);

                    // 画像を描画
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, pic.hDC, 0, 0, SRCCOPY);

                    withBlock.picMain(0).Refresh();
                    if (wait_time > 0)
                    {
                        do
                        {
                            Application.DoEvents();
                            cur_time = GeneralLib.timeGetTime();
                        }
                        while (start_time + wait_time > cur_time);
                        start_time = cur_time;
                    }
                }

                // ２回目の移動方向を設定
                if (Math.Abs((short)(x2 - x1)) > Math.Abs((short)(y2 - y1)))
                {
                    if (y2 > y1)
                    {
                        vy = 1;
                    }
                    else
                    {
                        vy = -1;
                    }

                    vx = 0;
                }
                else
                {
                    if (x2 > x1)
                    {
                        vx = 1;
                    }
                    else
                    {
                        vx = -1;
                    }

                    vy = 0;
                }

                // 移動の描画
                var loopTo1 = (short)(division * GeneralLib.MinLng(Math.Abs((short)(x2 - x1)), Math.Abs((short)(y2 - y1))));
                for (i = 1; i <= loopTo1; i++)
                {
                    // 画像を消去
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, withBlock.picMain(1).hDC, xx, yy, SRCCOPY);

                    // 座標を移動
                    xx = (short)(xx + 32 * vx / division);
                    yy = (short)(yy + 32 * vy / division);

                    // 画像を描画
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, pic.hDC, 0, 0, SRCCOPY);

                    withBlock.picMain(0).Refresh();
                    if (wait_time > 0)
                    {
                        do
                        {
                            Application.DoEvents();
                            cur_time = GeneralLib.timeGetTime();
                        }
                        while (start_time + wait_time > cur_time);
                        start_time = cur_time;
                    }
                }
            }

            // 画面が書き換えられたことを記録
            ScreenIsSaved = false;
        }

        // ユニット画像の表示位置を移動 (アニメーション)
        // 画像の経路を実際の移動経路にあわせる
        public static void MoveUnitBitmap2(ref Unit u, int wait_time0, short division = 2)
        {
            short xx, yy;
            short vx, vy;
            int ret;
            short i, j;
            PictureBox pic;
            int cur_time, start_time = default, wait_time;
            var PT = default(POINTAPI);
            var move_route_x = default(short[]);
            var move_route_y = default(short[]);
            wait_time = wait_time0 / division;
            SaveScreen();
            {
                var withBlock = MainForm;
                pic = withBlock.picTmp32(0);

                // ユニット画像を作成
                ret = BitBlt(pic.hDC, 0, 0, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);

                // ユニットのいる場所に合わせて表示を変更
                switch (u.Area ?? "")
                {
                    case "空中":
                        {
                            ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                            ret = LineTo(pic.hDC, 31, 28);
                            break;
                        }

                    case "水中":
                        {
                            ret = MoveToEx(pic.hDC, 0, 3, ref PT);
                            ret = LineTo(pic.hDC, 31, 3);
                            break;
                        }

                    case "地中":
                        {
                            ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                            ret = LineTo(pic.hDC, 31, 28);
                            ret = MoveToEx(pic.hDC, 0, 3, ref PT);
                            ret = LineTo(pic.hDC, 31, 3);
                            break;
                        }

                    case "宇宙":
                        {
                            if (Map.TerrainClass(u.x, u.y) == "月面")
                            {
                                ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                                ret = LineTo(pic.hDC, 31, 28);
                            }

                            break;
                        }
                }

                // 移動経路を検索
                Map.SearchMoveRoute(ref u.x, ref u.y, ref move_route_x, ref move_route_y);
                if (wait_time > 0)
                {
                    start_time = GeneralLib.timeGetTime();
                }

                // 移動の始点
                xx = MapToPixelX(move_route_x[Information.UBound(move_route_x)]);
                yy = MapToPixelY(move_route_y[Information.UBound(move_route_y)]);
                i = (short)(Information.UBound(move_route_x) - 1);
                while (i > 0)
                {
                    vx = (short)(MapToPixelX(move_route_x[i]) - xx);
                    vy = (short)(MapToPixelY(move_route_y[i]) - yy);

                    // 移動の描画
                    var loopTo = division;
                    for (j = 1; j <= loopTo; j++)
                    {
                        // 画像を消去
                        ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, withBlock.picMain(1).hDC, xx, yy, SRCCOPY);

                        // 座標を移動
                        xx = (short)(xx + (short)(vx / division));
                        yy = (short)(yy + (short)(vy / division));

                        // 画像を描画
                        ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, pic.hDC, 0, 0, SRCCOPY);

                        withBlock.picMain(0).Refresh();
                        if (wait_time > 0)
                        {
                            do
                            {
                                Application.DoEvents();
                                cur_time = GeneralLib.timeGetTime();
                            }
                            while (start_time + wait_time > cur_time);
                            start_time = cur_time;
                        }
                    }

                    i = (short)(i - 1);
                }
            }

            // 画面が書き換えられたことを記録
            ScreenIsSaved = false;
        }


        // === 各種リストボックスに関する処理 ===

        // リストボックスを表示
        public static short ListBox(ref string lb_caption, ref string[] list, ref string lb_info, [Optional, DefaultParameterValue("")] ref string lb_mode)
        {
            short ListBoxRet = default;
            short i;
            var is_rbutton_released = default(bool);

            Load(My.MyProject.Forms.frmListBox);
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                withBlock.WindowState = FormWindowState.Normal;

                // コメントウィンドウの処理
                if (Strings.InStr(lb_mode, "コメント") > 0)
                {
                    if (!withBlock.txtComment.Enabled)
                    {
                        withBlock.txtComment.Enabled = true;
                        withBlock.txtComment.Visible = true;
                        withBlock.txtComment.Width = withBlock.labCaption.Width;
                        withBlock.txtComment.Text = "";
                        withBlock.txtComment.Top = withBlock.lstItems.Top + withBlock.lstItems.Height + 5;
                        withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) + 600d);
                    }
                }
                else if (withBlock.txtComment.Enabled)
                {
                    withBlock.txtComment.Enabled = false;
                    withBlock.txtComment.Visible = false;
                    withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - 600d);
                }

                // キャプション
                withBlock.Text = lb_caption;
                if (Information.UBound(ListItemFlag) > 0)
                {
                    withBlock.labCaption.Text = "  " + lb_info;
                }
                else
                {
                    withBlock.labCaption.Text = lb_info;
                }

                // リストボックスにアイテムを追加
                withBlock.lstItems.Visible = false;
                withBlock.lstItems.Items.Clear();
                if (Information.UBound(ListItemFlag) > 0)
                {
                    var loopTo = (short)Information.UBound(list);
                    for (i = 1; i <= loopTo; i++)
                    {
                        if (ListItemFlag[i])
                        {
                            withBlock.lstItems.Items.Add("×" + list[i]);
                        }
                        else
                        {
                            withBlock.lstItems.Items.Add("  " + list[i]);
                        }
                    }

                    i = (short)Information.UBound(list);
                    while (i > 0)
                    {
                        if (!ListItemFlag[i])
                        {
                            withBlock.lstItems.SelectedIndex = i - 1;
                            break;
                        }

                        i = (short)(i - 1);
                    }
                }
                else
                {
                    var loopTo1 = (short)Information.UBound(list);
                    for (i = 1; i <= loopTo1; i++)
                        withBlock.lstItems.Items.Add(list[i]);
                }

                withBlock.lstItems.SelectedIndex = -1;
                withBlock.lstItems.Visible = true;

                // コメント付きのアイテム？
                if (Information.UBound(ListItemComment) != Information.UBound(list))
                {
                    Array.Resize(ref ListItemComment, Information.UBound(list) + 1);
                }

                // 最小化されている場合は戻しておく
                if (withBlock.WindowState != FormWindowState.Normal)
                {
                    withBlock.WindowState = FormWindowState.Normal;
                    withBlock.Show();
                }

                // 表示位置を設定
                if (MainForm.Visible & withBlock.HorizontalSize == "S")
                {
                    withBlock.Left = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(MainForm.Left));
                }
                else
                {
                    withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
                }

                if (MainForm.Visible & !((int)MainForm.WindowState == 1) & withBlock.VerticalSize == "M" & Strings.InStr(lb_mode, "中央表示") == 0)
                {
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(MainForm.Top) + SrcFormatter.PixelsToTwipsY(MainForm.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height));
                }
                else
                {
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);
                }

                // 先頭のアイテムを設定
                if (TopItem > 0)
                {
                    if (withBlock.lstItems.TopIndex != TopItem - 1)
                    {
                        withBlock.lstItems.TopIndex = GeneralLib.MaxLng(GeneralLib.MinLng(TopItem - 1, withBlock.lstItems.Items.Count - 1), 0);
                    }
                    if (withBlock.lstItems.Columns)
                    {
                        withBlock.lstItems.SelectedIndex = TopItem - 1;
                    }
                }
                else if (withBlock.lstItems.Columns)
                {
                    withBlock.lstItems.SelectedIndex = 0;
                }

                // コメントウィンドウの表示
                if (withBlock.txtComment.Enabled)
                {
                    withBlock.txtComment.Text = ListItemComment[withBlock.lstItems.SelectedIndex + 1];
                }

                Commands.SelectedItem = 0;
                IsFormClicked = false;
                Application.DoEvents();

                // リストボックスを表示
                if (Strings.InStr(lb_mode, "表示のみ") > 0)
                {
                    // 表示のみを行う
                    IsMordal = false;
                    withBlock.Show();
                    withBlock.lstItems.Focus();
                    SetWindowPos(withBlock.Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
                    withBlock.Refresh();
                    return ListBoxRet;
                }
                else if (Strings.InStr(lb_mode, "連続表示") > 0)
                {
                    // 選択が行われてもリストボックスを閉じない
                    IsMordal = false;
                    if (!withBlock.Visible)
                    {
                        withBlock.Show();
                        SetWindowPos(withBlock.Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
                        withBlock.lstItems.Focus();
                    }

                    if (Strings.InStr(lb_mode, "カーソル移動") > 0)
                    {
                        if (SRC.AutoMoveCursor)
                        {
                            string argcursor_mode1 = "ダイアログ";
                            MoveCursorPos(ref argcursor_mode1);
                        }
                    }

                    while (!IsFormClicked)
                    {
                        Application.DoEvents();
                        // 右ボタンでのダブルクリックの実現
                        if ((GetAsyncKeyState(RButtonID) & 0x8000) == 0)
                        {
                            is_rbutton_released = true;
                        }
                        else if (is_rbutton_released)
                        {
                            IsFormClicked = true;
                        }

                        Sleep(50);
                    }
                }
                else
                {
                    // 選択が行われた時点でリストボックスを閉じる
                    IsMordal = false;
                    withBlock.Show();
                    SetWindowPos(withBlock.Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
                    withBlock.lstItems.Focus();
                    if (Strings.InStr(lb_mode, "カーソル移動") > 0)
                    {
                        if (SRC.AutoMoveCursor)
                        {
                            string argcursor_mode = "ダイアログ";
                            MoveCursorPos(ref argcursor_mode);
                        }
                    }

                    while (!IsFormClicked)
                    {
                        Application.DoEvents();
                        // 右ボタンでのダブルクリックの実現
                        if ((GetAsyncKeyState(RButtonID) & 0x8000) == 0)
                        {
                            is_rbutton_released = true;
                        }
                        else if (is_rbutton_released)
                        {
                            IsFormClicked = true;
                        }

                        Sleep(50);
                    }

                    withBlock.Hide();
                    if (Strings.InStr(lb_mode, "カーソル移動") > 0 & Strings.InStr(lb_mode, "カーソル移動(行きのみ)") == 0)
                    {
                        if (SRC.AutoMoveCursor)
                        {
                            RestoreCursorPos();
                        }
                    }

                    if (withBlock.txtComment.Enabled)
                    {
                        withBlock.txtComment.Enabled = false;
                        withBlock.txtComment.Visible = false;
                        withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - 600d);
                    }
                }

                ListBoxRet = Commands.SelectedItem;
                Application.DoEvents();
            }

            return ListBoxRet;
        }

        // リストボックスの高さを大きくする
        public static void EnlargeListBoxHeight()
        {
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                switch (withBlock.VerticalSize ?? "")
                {
                    case "M":
                        {
                            if (withBlock.WindowState != FormWindowState.Normal)
                            {
                                withBlock.Visible = true;
                                withBlock.WindowState = FormWindowState.Normal;
                            }

                            withBlock.Visible = false;
                            withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) + 2400d);
                            withBlock.lstItems.Height = 260;
                            withBlock.VerticalSize = "L";
                            break;
                        }
                }
            }
        }

        // リストボックスの高さを小さくする
        public static void ReduceListBoxHeight()
        {
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                switch (withBlock.VerticalSize ?? "")
                {
                    case "L":
                        {
                            if (withBlock.WindowState != FormWindowState.Normal)
                            {
                                withBlock.Visible = true;
                                withBlock.WindowState = FormWindowState.Normal;
                            }

                            withBlock.Visible = false;
                            withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - 2400d);
                            withBlock.lstItems.Height = 100;
                            withBlock.VerticalSize = "M";
                            break;
                        }
                }
            }
        }

        // リストボックスの幅を大きくする
        public static void EnlargeListBoxWidth()
        {
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                switch (withBlock.HorizontalSize ?? "")
                {
                    case "S":
                        {
                            if (withBlock.WindowState != FormWindowState.Normal)
                            {
                                withBlock.Visible = true;
                                withBlock.WindowState = FormWindowState.Normal;
                            }

                            withBlock.Visible = false;
                            withBlock.Width = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(withBlock.Width) + 2350d);
                            withBlock.lstItems.Width = 637;
                            withBlock.labCaption.Width = 637;
                            withBlock.HorizontalSize = "M";
                            break;
                        }
                }
            }
        }

        // リストボックスの幅を小さくする
        public static void ReduceListBoxWidth()
        {
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                switch (withBlock.HorizontalSize ?? "")
                {
                    case "M":
                        {
                            if (withBlock.WindowState != FormWindowState.Normal)
                            {
                                withBlock.Visible = true;
                                withBlock.WindowState = FormWindowState.Normal;
                            }

                            withBlock.Visible = false;
                            withBlock.Width = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(withBlock.Width) - 2350d);
                            withBlock.lstItems.Width = 486;
                            withBlock.labCaption.Width = 486;
                            withBlock.HorizontalSize = "S";
                            break;
                        }
                }
            }
        }

        // 武器選択用にリストボックスを切り替え
        public static void AddPartsToListBox()
        {
            int ret;
            string fname;
            Unit u, t;
            u = Commands.SelectedUnit;
            t = Commands.SelectedTarget;
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                // リストボックスにユニットやＨＰのゲージを追加
                withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) + 535d);
                withBlock.labCaption.Top = 42;
                withBlock.lstItems.Top = 69;
                withBlock.imgPilot1.Visible = true;
                withBlock.labLevel1.Visible = true;
                withBlock.txtLevel1.Visible = true;
                withBlock.labMorale1.Visible = true;
                withBlock.txtMorale1.Visible = true;
                withBlock.picUnit1.Visible = true;
                withBlock.labHP1.Visible = true;
                withBlock.txtHP1.Visible = true;
                withBlock.picHP1.Visible = true;
                withBlock.labEN1.Visible = true;
                withBlock.txtEN1.Visible = true;
                withBlock.picEN1.Visible = true;
                withBlock.imgPilot2.Visible = true;
                withBlock.labLevel2.Visible = true;
                withBlock.txtLevel2.Visible = true;
                withBlock.labMorale2.Visible = true;
                withBlock.txtMorale2.Visible = true;
                withBlock.picUnit2.Visible = true;
                withBlock.labHP2.Visible = true;
                withBlock.txtHP2.Visible = true;
                withBlock.picHP2.Visible = true;
                withBlock.labEN2.Visible = true;
                withBlock.txtEN2.Visible = true;
                withBlock.picEN2.Visible = true;

                // ユニット側の表示
                fname = @"Bitmap\Pilot\" + u.MainPilot().get_Bitmap(false);
                bool localFileExists() { string argfname = SRC.ExtDataPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists1() { string argfname = SRC.ExtDataPath2 + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists2() { string argfname = SRC.AppPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                string argfname = SRC.ScenarioPath + fname;
                if (GeneralLib.FileExists(ref argfname))
                {
                    withBlock.imgPilot1.Image = Image.FromFile(SRC.ScenarioPath + fname);
                }
                else if (localFileExists())
                {
                    withBlock.imgPilot1.Image = Image.FromFile(SRC.ExtDataPath + fname);
                }
                else if (localFileExists1())
                {
                    withBlock.imgPilot1.Image = Image.FromFile(SRC.ExtDataPath2 + fname);
                }
                else if (localFileExists2())
                {
                    withBlock.imgPilot1.Image = Image.FromFile(SRC.AppPath + fname);
                }
                else
                {
                    withBlock.imgPilot1.Image = Image.FromFile("");
                }

                withBlock.txtLevel1.Text = SrcFormatter.Format(u.MainPilot().Level);
                withBlock.txtMorale1.Text = SrcFormatter.Format(u.MainPilot().Morale);
                if (string.IsNullOrEmpty(Map.MapDrawMode))
                {
                    if (u.BitmapID > 0)
                    {
                        ret = BitBlt(withBlock.picUnit1.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);
                    }
                    else
                    {
                        // 非表示のユニットの場合はユニットのいる地形タイルを表示
                        ret = BitBlt(withBlock.picUnit1.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * ((int)u.x - 1), 32 * ((int)u.y - 1), SRCCOPY);
                    }
                }
                else
                {
                    var argpic = withBlock.picUnit1;
                    string argfname1 = "";
                    LoadUnitBitmap(ref u, ref argpic, 0, 0, true, fname: ref argfname1);
                    withBlock.picUnit1 = argpic;
                }

                withBlock.picUnit1.Refresh();
                object argIndex1 = "データ不明";
                if (u.IsConditionSatisfied(ref argIndex1))
                {
                    string argtname = "HP";
                    Unit argu = null;
                    withBlock.labHP1.Text = Expression.Term(ref argtname, u: ref argu);
                    withBlock.txtHP1.Text = "?????/?????";
                }
                else
                {
                    string argtname1 = "HP";
                    Unit argu1 = null;
                    withBlock.labHP1.Text = Expression.Term(ref argtname1, u: ref argu1);
                    if (u.HP < 100000)
                    {
                        withBlock.txtHP1.Text = SrcFormatter.Format(u.HP);
                    }
                    else
                    {
                        withBlock.txtHP1.Text = "?????";
                    }

                    if (u.MaxHP < 100000)
                    {
                        withBlock.txtHP1.Text = withBlock.txtHP1.Text + "/" + SrcFormatter.Format(u.MaxHP);
                    }
                    else
                    {
                        withBlock.txtHP1.Text = withBlock.txtHP1.Text + "/?????";
                    }
                }
                withBlock.picHP1.Cls();
            }
            My.MyProject.Forms.frmListBox.picHP1.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            {
                var withBlock1 = My.MyProject.Forms.frmListBox;
                object argIndex2 = "データ不明";
                if (u.IsConditionSatisfied(ref argIndex2))
                {
                    string argtname2 = "EN";
                    Unit argu2 = null;
                    withBlock1.labEN1.Text = Expression.Term(ref argtname2, u: ref argu2);
                    withBlock1.txtEN1.Text = "???/???";
                }
                else
                {
                    string argtname3 = "EN";
                    withBlock1.labEN1.Text = Expression.Term(ref argtname3, ref t);
                    if (u.EN < 1000)
                    {
                        withBlock1.txtEN1.Text = SrcFormatter.Format(u.EN);
                    }
                    else
                    {
                        withBlock1.txtEN1.Text = "???";
                    }

                    if (u.MaxEN < 1000)
                    {
                        withBlock1.txtEN1.Text = withBlock1.txtEN1.Text + "/" + SrcFormatter.Format(u.MaxEN);
                    }
                    else
                    {
                        withBlock1.txtEN1.Text = withBlock1.txtEN1.Text + "/???";
                    }
                }
                withBlock1.picEN1.Cls();
            }
            My.MyProject.Forms.frmListBox.picEN1.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            {
                var withBlock2 = My.MyProject.Forms.frmListBox;
                // ターゲット側の表示
                fname = @"Bitmap\Pilot\" + t.MainPilot().get_Bitmap(false);
                bool localFileExists3() { string argfname = SRC.ExtDataPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists4() { string argfname = SRC.ExtDataPath2 + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists5() { string argfname = SRC.AppPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                string argfname2 = SRC.ScenarioPath + fname;
                if (GeneralLib.FileExists(ref argfname2))
                {
                    withBlock2.imgPilot2.Image = Image.FromFile(SRC.ScenarioPath + fname);
                }
                else if (localFileExists3())
                {
                    withBlock2.imgPilot2.Image = Image.FromFile(SRC.ExtDataPath + fname);
                }
                else if (localFileExists4())
                {
                    withBlock2.imgPilot2.Image = Image.FromFile(SRC.ExtDataPath2 + fname);
                }
                else if (localFileExists5())
                {
                    withBlock2.imgPilot2.Image = Image.FromFile(SRC.AppPath + fname);
                }
                else
                {
                    withBlock2.imgPilot2.Image = Image.FromFile("");
                }

                withBlock2.txtLevel2.Text = SrcFormatter.Format(t.MainPilot().Level);
                withBlock2.txtMorale2.Text = SrcFormatter.Format(t.MainPilot().Morale);
                if (string.IsNullOrEmpty(Map.MapDrawMode))
                {
                    if (t.BitmapID > 0)
                    {
                        ret = BitBlt(withBlock2.picUnit2.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * ((int)t.BitmapID % 15), 96 * ((int)t.BitmapID / 15), SRCCOPY);
                    }
                    else
                    {
                        // 非表示のユニットの場合はユニットのいる地形タイルを表示
                        ret = BitBlt(withBlock2.picUnit2.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * ((int)t.x - 1), 32 * ((int)t.y - 1), SRCCOPY);
                    }
                }
                else
                {
                    var argpic1 = withBlock2.picUnit2;
                    string argfname3 = "";
                    LoadUnitBitmap(ref t, ref argpic1, 0, 0, true, fname: ref argfname3);
                    withBlock2.picUnit2 = argpic1;
                }

                withBlock2.picUnit2.Refresh();
                object argIndex3 = "データ不明";
                if (t.IsConditionSatisfied(ref argIndex3))
                {
                    string argtname4 = "HP";
                    Unit argu3 = null;
                    withBlock2.labHP2.Text = Expression.Term(ref argtname4, u: ref argu3);
                    withBlock2.txtHP2.Text = "?????/?????";
                }
                else
                {
                    string argtname5 = "HP";
                    withBlock2.labHP2.Text = Expression.Term(ref argtname5, ref t);
                    if (t.HP < 100000)
                    {
                        withBlock2.txtHP2.Text = SrcFormatter.Format(t.HP);
                    }
                    else
                    {
                        withBlock2.txtHP2.Text = "?????";
                    }

                    if (t.MaxHP < 100000)
                    {
                        withBlock2.txtHP2.Text = withBlock2.txtHP2.Text + "/" + SrcFormatter.Format(t.MaxHP);
                    }
                    else
                    {
                        withBlock2.txtHP2.Text = withBlock2.txtHP2.Text + "/?????";
                    }
                }
                withBlock2.picHP2.Cls();
            }
            My.MyProject.Forms.frmListBox.picHP2.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            {
                var withBlock3 = My.MyProject.Forms.frmListBox;
                object argIndex4 = "データ不明";
                if (t.IsConditionSatisfied(ref argIndex4))
                {
                    string argtname6 = "EN";
                    Unit argu4 = null;
                    withBlock3.labEN2.Text = Expression.Term(ref argtname6, u: ref argu4);
                    withBlock3.txtEN2.Text = "???/???";
                }
                else
                {
                    string argtname7 = "EN";
                    withBlock3.labEN2.Text = Expression.Term(ref argtname7, ref t);
                    if (t.EN < 1000)
                    {
                        withBlock3.txtEN2.Text = SrcFormatter.Format(t.EN);
                    }
                    else
                    {
                        withBlock3.txtEN2.Text = "???";
                    }

                    if (t.MaxEN < 1000)
                    {
                        withBlock3.txtEN2.Text = withBlock3.txtEN2.Text + "/" + SrcFormatter.Format(t.MaxEN);
                    }
                    else
                    {
                        withBlock3.txtEN2.Text = withBlock3.txtEN2.Text + "/???";
                    }
                }
                withBlock3.picEN2.Cls();
            }
            My.MyProject.Forms.frmListBox.picEN2.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
        }

        // 武器選択用リストボックスを通常のものに切り替え
        public static void RemovePartsOnListBox()
        {
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock.Height) - 535d);
                withBlock.labCaption.Top = 4;
                withBlock.lstItems.Top = 32;
                withBlock.imgPilot1.Visible = false;
                withBlock.labLevel1.Visible = false;
                withBlock.txtLevel1.Visible = false;
                withBlock.labMorale1.Visible = false;
                withBlock.txtMorale1.Visible = false;
                withBlock.picUnit1.Visible = false;
                withBlock.labHP1.Visible = false;
                withBlock.txtHP1.Visible = false;
                withBlock.picHP1.Visible = false;
                withBlock.labEN1.Visible = false;
                withBlock.txtEN1.Visible = false;
                withBlock.picEN1.Visible = false;
                withBlock.imgPilot2.Visible = false;
                withBlock.labLevel2.Visible = false;
                withBlock.txtLevel2.Visible = false;
                withBlock.labMorale2.Visible = false;
                withBlock.txtMorale2.Visible = false;
                withBlock.picUnit2.Visible = false;
                withBlock.labHP2.Visible = false;
                withBlock.txtHP2.Visible = false;
                withBlock.picHP2.Visible = false;
                withBlock.labEN2.Visible = false;
                withBlock.txtEN2.Visible = false;
                withBlock.picEN2.Visible = false;
            }
        }

        // 武器選択用リストボックス
        public static short WeaponListBox(ref Unit u, ref string caption_msg, ref string lb_mode, [Optional, DefaultParameterValue("")] ref string BGM)
        {
            short WeaponListBoxRet = default;
            short ret, j, i, k, w;
            string[] list;
            short[] wlist;
            short[] warray;
            int[] wpower;
            string wclass;
            var is_rbutton_released = default(bool);
            string buf;
            {
                var withBlock = u;
                warray = new short[(withBlock.CountWeapon() + 1)];
                wpower = new int[(withBlock.CountWeapon() + 1)];
                ListItemFlag = new bool[(withBlock.CountWeapon() + 1)];
                var ToolTips = new object[(withBlock.CountWeapon() + 1)];
                var loopTo = withBlock.CountWeapon();
                for (i = 1; i <= loopTo; i++)
                {
                    string argtarea = "";
                    wpower[i] = withBlock.WeaponPower(i, ref argtarea);
                }

                // 攻撃力でソート
                var loopTo1 = withBlock.CountWeapon();
                for (i = 1; i <= loopTo1; i++)
                {
                    var loopTo2 = (short)(i - 1);
                    for (j = 1; j <= loopTo2; j++)
                    {
                        if (wpower[i] > wpower[warray[i - j]])
                        {
                            break;
                        }
                        else if (wpower[i] == wpower[warray[i - j]])
                        {
                            if (withBlock.Weapon(i).ENConsumption > 0)
                            {
                                if (withBlock.Weapon(i).ENConsumption >= withBlock.Weapon(warray[i - j]).ENConsumption)
                                {
                                    break;
                                }
                            }
                            else if (withBlock.Weapon(i).Bullet > 0)
                            {
                                if (withBlock.Weapon(i).Bullet <= withBlock.Weapon(warray[i - j]).Bullet)
                                {
                                    break;
                                }
                            }
                            else if (withBlock.Weapon((short)(i - j)).ENConsumption == 0 & withBlock.Weapon(warray[i - j]).Bullet == 0)
                            {
                                break;
                            }
                        }
                    }

                    var loopTo3 = (short)(j - 1);
                    for (k = 1; k <= loopTo3; k++)
                        warray[i - k + 1] = warray[i - k];
                    warray[i - j + 1] = i;
                }
            }

            list = new string[1];
            wlist = new short[1];
            if (lb_mode == "移動前" | lb_mode == "移動後" | lb_mode == "一覧")
            {
                // 通常の武器選択時の表示
                var loopTo4 = u.CountWeapon();
                for (i = 1; i <= loopTo4; i++)
                {
                    w = warray[i];
                    {
                        var withBlock1 = u;
                        if (lb_mode == "一覧")
                        {
                            string argref_mode = "ステータス";
                            if (!withBlock1.IsWeaponAvailable(w, ref argref_mode))
                            {
                                // Disableコマンドで使用不可にされた武器と使用できない合体技
                                // は表示しない
                                if (withBlock1.IsDisabled(ref withBlock1.Weapon(w).Name))
                                {
                                    goto NextLoop1;
                                }

                                if (!withBlock1.IsWeaponMastered(w))
                                {
                                    goto NextLoop1;
                                }

                                string argattr = "合";
                                if (withBlock1.IsWeaponClassifiedAs(w, ref argattr))
                                {
                                    if (!withBlock1.IsCombinationAttackAvailable(w, true))
                                    {
                                        goto NextLoop1;
                                    }
                                }
                            }

                            ListItemFlag[Information.UBound(list) + 1] = false;
                        }
                        else if (withBlock1.IsWeaponUseful(w, ref lb_mode))
                        {
                            ListItemFlag[Information.UBound(list) + 1] = false;
                        }
                        else
                        {
                            // Disableコマンドで使用不可にされた武器と使用できない合体技
                            // は表示しない
                            if (withBlock1.IsDisabled(ref withBlock1.Weapon(w).Name))
                            {
                                goto NextLoop1;
                            }

                            if (!withBlock1.IsWeaponMastered(w))
                            {
                                goto NextLoop1;
                            }

                            string argattr1 = "合";
                            if (withBlock1.IsWeaponClassifiedAs(w, ref argattr1))
                            {
                                if (!withBlock1.IsCombinationAttackAvailable(w, true))
                                {
                                    goto NextLoop1;
                                }
                            }

                            ListItemFlag[Information.UBound(list) + 1] = true;
                        }
                    }

                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref wlist, Information.UBound(list) + 1);
                    wlist[Information.UBound(list)] = w;

                    // 各武器の表示内容の設定
                    {
                        var withBlock2 = u.Weapon(w);
                        // 攻撃力
                        if (wpower[w] < 10000)
                        {
                            string localLeftPaddedString() { string argbuf = SrcFormatter.Format(wpower[w]); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref withBlock2.Nickname(), 27) + localLeftPaddedString();
                        }
                        else
                        {
                            string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(wpower[w]); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref withBlock2.Nickname(), 26) + localLeftPaddedString1();
                        }

                        // 最大射程
                        if (u.WeaponMaxRange(w) > 1)
                        {
                            buf = SrcFormatter.Format(withBlock2.MinRange) + "-" + SrcFormatter.Format(u.WeaponMaxRange(w));
                            list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.LeftPaddedString(ref buf, 5);
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "    1";
                        }

                        // 命中率修正
                        if (u.WeaponPrecision(w) >= 0)
                        {
                            string localLeftPaddedString2() { string argbuf = "+" + SrcFormatter.Format(u.WeaponPrecision(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString2();
                        }
                        else
                        {
                            string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(u.WeaponPrecision(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString3();
                        }

                        // 残り弾数
                        if (withBlock2.Bullet > 0)
                        {
                            string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(u.Bullet(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString4();
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "  -";
                        }

                        // ＥＮ消費量
                        if (withBlock2.ENConsumption > 0)
                        {
                            string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(u.WeaponENConsumption(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString5();
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "   -";
                        }

                        // クリティカル率修正
                        if (u.WeaponCritical(w) >= 0)
                        {
                            string localLeftPaddedString6() { string argbuf = "+" + SrcFormatter.Format(u.WeaponCritical(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString6();
                        }
                        else
                        {
                            string localLeftPaddedString7() { string argbuf = SrcFormatter.Format(u.WeaponCritical(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString7();
                        }

                        // 地形適応
                        list[Information.UBound(list)] = list[Information.UBound(list)] + " " + withBlock2.Adaption;

                        // 必要気力
                        if (withBlock2.NecessaryMorale > 0)
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + " 気" + withBlock2.NecessaryMorale;
                        }

                        // 属性
                        wclass = u.WeaponClass(w);
                        string argstring21 = "|";
                        if (GeneralLib.InStrNotNest(ref wclass, ref argstring21) > 0)
                        {
                            string argstring2 = "|";
                            wclass = Strings.Left(wclass, GeneralLib.InStrNotNest(ref wclass, ref argstring2) - 1);
                        }

                        list[Information.UBound(list)] = list[Information.UBound(list)] + " " + wclass;
                    }

                    NextLoop1:
                    ;
                }

                if (lb_mode == "移動前" | lb_mode == "移動後")
                {
                    Unit argt = null;
                    Unit argt1 = null;
                    if (u.LookForSupportAttack(ref argt1) is object)
                    {
                        // 援護攻撃を使うかどうか選択
                        Commands.UseSupportAttack = true;
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref ListItemFlag, Information.UBound(list) + 1);
                        list[Information.UBound(list)] = "援護攻撃：使用する";
                    }
                }

                // リストボックスを表示
                TopItem = -1;
                string argtname = "EN";
                string argtname1 = "CT";
                string arglb_info = "名称                       攻撃 射程  命 弾  " + Expression.Term(ref argtname, ref u, 2) + "  " + Expression.Term(ref argtname1, ref u, 2) + " 適応 分類";
                string arglb_mode = "表示のみ";
                ret = ListBox(ref caption_msg, ref list, ref arglb_info, ref arglb_mode);
                if (SRC.AutoMoveCursor)
                {
                    if (lb_mode != "一覧")
                    {
                        string argcursor_mode = "武器選択";
                        MoveCursorPos(ref argcursor_mode);
                    }
                    else
                    {
                        string argcursor_mode1 = "ダイアログ";
                        MoveCursorPos(ref argcursor_mode1);
                    }
                }

                if (!string.IsNullOrEmpty(BGM))
                {
                    Sound.ChangeBGM(ref BGM);
                }

                while (true)
                {
                    while (!IsFormClicked)
                    {
                        Application.DoEvents();
                        // 右ボタンでのダブルクリックの実現
                        if ((GetAsyncKeyState(RButtonID) & 0x8000) == 0)
                        {
                            is_rbutton_released = true;
                        }
                        else if (is_rbutton_released)
                        {
                            IsFormClicked = true;
                        }
                    }

                    if (Commands.SelectedItem <= Information.UBound(wlist))
                    {
                        break;
                    }
                    else
                    {
                        // 援護攻撃のオン/オフ切り替え
                        Commands.UseSupportAttack = !Commands.UseSupportAttack;
                        if (Commands.UseSupportAttack)
                        {
                            list[Information.UBound(list)] = "援護攻撃：使用する";
                        }
                        else
                        {
                            list[Information.UBound(list)] = "援護攻撃：使用しない";
                        }

                        string argtname2 = "EN";
                        string argtname3 = "CT";
                        string arglb_info1 = "名称                       攻撃 射程  命 弾  " + Expression.Term(ref argtname2, ref u, 2) + "  " + Expression.Term(ref argtname3, ref u, 2) + " 適応 分類";
                        string arglb_mode1 = "表示のみ";
                        Commands.SelectedItem = ListBox(ref caption_msg, ref list, ref arglb_info1, ref arglb_mode1);
                    }
                }

                if (lb_mode != "一覧")
                {
                    My.MyProject.Forms.frmListBox.Hide();
                }

                ListItemComment = new string[1];
                WeaponListBoxRet = wlist[Commands.SelectedItem];
            }
            else if (lb_mode == "反撃")
            {
                // 反撃武器選択時の表示

                var loopTo5 = u.CountWeapon();
                for (i = 1; i <= loopTo5; i++)
                {
                    w = warray[i];
                    {
                        var withBlock3 = u;
                        // Disableコマンドで使用不可にされた武器は表示しない
                        if (withBlock3.IsDisabled(ref withBlock3.Weapon(w).Name))
                        {
                            goto NextLoop2;
                        }

                        // 必要技能を満たさない武器は表示しない
                        if (!withBlock3.IsWeaponMastered(w))
                        {
                            goto NextLoop2;
                        }

                        // 使用できない合体技は表示しない
                        string argattr2 = "合";
                        if (withBlock3.IsWeaponClassifiedAs(w, ref argattr2))
                        {
                            if (!withBlock3.IsCombinationAttackAvailable(w, true))
                            {
                                goto NextLoop2;
                            }
                        }

                        string argref_mode1 = "移動前";
                        string argattr3 = "Ｍ";
                        string argattr4 = "合";
                        if (!withBlock3.IsWeaponAvailable(w, ref argref_mode1))
                        {
                            // この武器は使用不能
                            ListItemFlag[Information.UBound(list) + 1] = true;
                        }
                        else if (!withBlock3.IsTargetWithinRange(w, ref Commands.SelectedUnit))
                        {
                            // ターゲットが射程外
                            ListItemFlag[Information.UBound(list) + 1] = true;
                        }
                        else if (withBlock3.IsWeaponClassifiedAs(w, ref argattr3))
                        {
                            // マップ攻撃は武器選定外
                            ListItemFlag[Information.UBound(list) + 1] = true;
                        }
                        else if (withBlock3.IsWeaponClassifiedAs(w, ref argattr4))
                        {
                            // 合体技は自分から攻撃をかける場合にのみ使用
                            ListItemFlag[Information.UBound(list) + 1] = true;
                        }
                        else if (withBlock3.Damage(w, ref Commands.SelectedUnit, true) > 0)
                        {
                            // ダメージを与えられる
                            ListItemFlag[Information.UBound(list) + 1] = false;
                        }
                        else if (!withBlock3.IsNormalWeapon(w) & withBlock3.CriticalProbability(w, ref Commands.SelectedUnit) > 0)
                        {
                            // 特殊効果を与えられる
                            ListItemFlag[Information.UBound(list) + 1] = false;
                        }
                        else
                        {
                            // この武器は効果が無い
                            ListItemFlag[Information.UBound(list) + 1] = true;
                        }
                    }

                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref wlist, Information.UBound(list) + 1);
                    wlist[Information.UBound(list)] = w;

                    // 各武器の表示内容の設定
                    {
                        var withBlock4 = u.Weapon(w);
                        // 攻撃力
                        string localLeftPaddedString8() { string argbuf = SrcFormatter.Format(wpower[w]); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                        list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref withBlock4.Nickname(), 29) + localLeftPaddedString8();

                        // 命中率
                        string argoname = "予測命中率非表示";
                        if (!Expression.IsOptionDefined(ref argoname))
                        {
                            buf = SrcFormatter.Format(GeneralLib.MinLng(u.HitProbability(w, ref Commands.SelectedUnit, true), 100)) + "%";
                            list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.LeftPaddedString(ref buf, 5);
                        }
                        else if (u.WeaponPrecision(w) >= 0)
                        {
                            string localLeftPaddedString10() { string argbuf = "+" + SrcFormatter.Format(u.WeaponPrecision(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString10();
                        }
                        else
                        {
                            string localLeftPaddedString9() { string argbuf = SrcFormatter.Format(u.WeaponPrecision(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString9();
                        }


                        // クリティカル率
                        string argoname1 = "予測命中率非表示";
                        if (!Expression.IsOptionDefined(ref argoname1))
                        {
                            buf = SrcFormatter.Format(GeneralLib.MinLng(u.CriticalProbability(w, ref Commands.SelectedUnit), 100)) + "%";
                            list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.LeftPaddedString(ref buf, 5);
                        }
                        else if (u.WeaponCritical(w) >= 0)
                        {
                            string localLeftPaddedString12() { string argbuf = "+" + SrcFormatter.Format(u.WeaponCritical(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString12();
                        }
                        else
                        {
                            string localLeftPaddedString11() { string argbuf = SrcFormatter.Format(u.WeaponCritical(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString11();
                        }

                        // 残り弾数
                        if (withBlock4.Bullet > 0)
                        {
                            string localLeftPaddedString13() { string argbuf = SrcFormatter.Format(u.Bullet(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString13();
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "  -";
                        }

                        // ＥＮ消費量
                        if (withBlock4.ENConsumption > 0)
                        {
                            string localLeftPaddedString14() { string argbuf = SrcFormatter.Format(u.WeaponENConsumption(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString14();
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "   -";
                        }

                        // 地形適応
                        list[Information.UBound(list)] = list[Information.UBound(list)] + " " + withBlock4.Adaption;

                        // 必要気力
                        if (withBlock4.NecessaryMorale > 0)
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + " 気" + withBlock4.NecessaryMorale;
                        }

                        // 属性
                        wclass = u.WeaponClass(w);
                        string argstring23 = "|";
                        if (GeneralLib.InStrNotNest(ref wclass, ref argstring23) > 0)
                        {
                            string argstring22 = "|";
                            wclass = Strings.Left(wclass, GeneralLib.InStrNotNest(ref wclass, ref argstring22) - 1);
                        }

                        list[Information.UBound(list)] = list[Information.UBound(list)] + " " + wclass;
                    }

                    NextLoop2:
                    ;
                }

                // リストボックスを表示
                TopItem = -1;
                string argtname4 = "CT";
                string argtname5 = "EN";
                string arglb_info2 = "名称                         攻撃 命中 " + Expression.Term(ref argtname4, ref u, 2) + "   弾  " + Expression.Term(ref argtname5, ref u, 2) + " 適応 分類";
                string arglb_mode2 = "連続表示,カーソル移動";
                ret = ListBox(ref caption_msg, ref list, ref arglb_info2, ref arglb_mode2);
                WeaponListBoxRet = wlist[ret];
            }

            Application.DoEvents();
            return WeaponListBoxRet;
        }

        // アビリティ選択用リストボックス
        public static short AbilityListBox(ref Unit u, ref string caption_msg, ref string lb_mode, bool is_item = false)
        {
            short AbilityListBoxRet = default;
            short j, i, k;
            short ret;
            string msg, buf, rest_msg;
            string[] list;
            var alist = default(short[]);
            bool is_available;
            var is_rbutton_released = default(bool);
            {
                var withBlock = u;
                // アビリティが一つしかない場合は自動的にそのアビリティを選択する。
                // リストボックスの表示は行わない。
                string argtname = "アビリティ";
                if (lb_mode != "一覧" & !is_item & MainForm.mnuUnitCommandItem(Commands.AbilityCmdID).Caption != Expression.Term(ref argtname, ref u))
                {
                    var loopTo = withBlock.CountAbility();
                    for (i = 1; i <= loopTo; i++)
                    {
                        if (!withBlock.Ability(i).IsItem() & withBlock.IsAbilityMastered(i))
                        {
                            AbilityListBoxRet = i;
                            return AbilityListBoxRet;
                        }
                    }
                }

                list = new string[1];
                var aist = new object[1];
                ListItemFlag = new bool[1];
                var loopTo1 = withBlock.CountAbility();
                for (i = 1; i <= loopTo1; i++)
                {
                    is_available = true;
                    if (lb_mode == "一覧")
                    {
                        string argref_mode = "ステータス";
                        if (withBlock.IsAbilityAvailable(i, ref argref_mode))
                        {
                            // アイテムの使用効果かどうか
                            {
                                var withBlock1 = withBlock.Ability(i);
                                if (is_item)
                                {
                                    if (!withBlock1.IsItem())
                                    {
                                        goto NextLoop;
                                    }
                                }
                                else if (withBlock1.IsItem())
                                {
                                    goto NextLoop;
                                }
                            }
                        }
                        else
                        {
                            // Disableコマンドで使用不可にされたアビリティと使用できない合体技
                            // は表示しない
                            if (withBlock.IsDisabled(ref withBlock.Ability(i).Name))
                            {
                                goto NextLoop;
                            }

                            if (!withBlock.IsAbilityMastered(i))
                            {
                                goto NextLoop;
                            }

                            string argattr = "合";
                            if (withBlock.IsAbilityClassifiedAs(i, ref argattr))
                            {
                                if (!withBlock.IsCombinationAbilityAvailable(i, true))
                                {
                                    goto NextLoop;
                                }
                            }
                        }
                    }
                    else
                    {
                        // アイテムの使用効果かどうか
                        {
                            var withBlock2 = withBlock.Ability(i);
                            if (is_item)
                            {
                                if (!withBlock2.IsItem())
                                {
                                    goto NextLoop;
                                }
                            }
                            else if (withBlock2.IsItem())
                            {
                                goto NextLoop;
                            }
                        }

                        if (!withBlock.IsAbilityUseful(i, ref lb_mode))
                        {
                            // Disableコマンドで使用不可にされた武器と使用できない合体技
                            // は表示しない
                            if (withBlock.IsDisabled(ref withBlock.Ability(i).Name))
                            {
                                goto NextLoop;
                            }

                            if (!withBlock.IsAbilityMastered(i))
                            {
                                goto NextLoop;
                            }

                            string argattr1 = "合";
                            if (withBlock.IsAbilityClassifiedAs(i, ref argattr1))
                            {
                                if (!withBlock.IsCombinationAbilityAvailable(i, true))
                                {
                                    goto NextLoop;
                                }
                            }

                            is_available = false;
                        }
                    }

                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref alist, Information.UBound(list) + 1);
                    Array.Resize(ref ListItemFlag, Information.UBound(list) + 1);
                    alist[Information.UBound(list)] = i;
                    ListItemFlag[Information.UBound(list)] = !is_available;
                    {
                        var withBlock3 = withBlock.Ability(i);
                        list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref withBlock3.Nickname(), 20);
                        msg = "";
                        rest_msg = "";
                        var loopTo2 = withBlock3.CountEffect();
                        for (j = 1; j <= loopTo2; j++)
                        {
                            string localEffectName6() { object argIndex1 = j; var ret = withBlock3.EffectName(ref argIndex1); return ret; }

                            string localEffectName7() { object argIndex1 = j; var ret = withBlock3.EffectName(ref argIndex1); return ret; }

                            object argIndex2 = j;
                            if (withBlock3.EffectType(ref argIndex2) == "解説")
                            {
                                object argIndex1 = j;
                                msg = withBlock3.EffectName(ref argIndex1);
                                break;
                            }
                            else if (Strings.InStr(localEffectName6(), "ターン)") > 0)
                            {
                                // 持続時間が同じ能力はターン数をまとめて表示
                                string localEffectName() { object argIndex1 = j; var ret = withBlock3.EffectName(ref argIndex1); return ret; }

                                string localEffectName1() { object argIndex1 = j; var ret = withBlock3.EffectName(ref argIndex1); return ret; }

                                k = (short)Strings.InStr(msg, Strings.Mid(localEffectName(), Strings.InStr(localEffectName1(), "(")));
                                if (k > 0)
                                {
                                    string localEffectName2() { object argIndex1 = j; var ret = withBlock3.EffectName(ref argIndex1); return ret; }

                                    string localEffectName3() { object argIndex1 = j; var ret = withBlock3.EffectName(ref argIndex1); return ret; }

                                    msg = Strings.Left(msg, k - 1) + "、" + Strings.Left(localEffectName2(), Strings.InStr(localEffectName3(), "(") - 1) + Strings.Mid(msg, k);
                                }
                                else
                                {
                                    string localEffectName4() { object argIndex1 = j; var ret = withBlock3.EffectName(ref argIndex1); return ret; }

                                    msg = msg + " " + localEffectName4();
                                }
                            }
                            else if (!string.IsNullOrEmpty(localEffectName7()))
                            {
                                string localEffectName5() { object argIndex1 = j; var ret = withBlock3.EffectName(ref argIndex1); return ret; }

                                msg = msg + " " + localEffectName5();
                            }
                        }

                        msg = Strings.Trim(msg);

                        // 効果解説が長すぎる場合は改行
                        buf = Strings.StrConv(msg, vbFromUnicode);
                        if (LenB(buf) > 32)
                        {
                            do
                            {
                                buf = Strings.StrConv(buf, vbUnicode);
                                buf = Strings.Left(buf, Strings.Len(buf) - 1);
                                buf = Strings.StrConv(buf, vbFromUnicode);
                            }
                            while (LenB(buf) >= 32);
                            buf = Strings.StrConv(buf, vbUnicode);
                            rest_msg = Strings.Mid(msg, Strings.Len(buf) + 1);
                            if (LenB(Strings.StrConv(buf, vbFromUnicode)) < 32)
                            {
                                buf = buf + Strings.Space(32 - LenB(Strings.StrConv(buf, vbFromUnicode)));
                            }

                            msg = buf;
                        }

                        string argbuf = list[Information.UBound(list)] + " " + msg;
                        list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf, 53);

                        // 最大射程
                        if (u.AbilityMaxRange(i) > 1)
                        {
                            string localLeftPaddedString() { string argbuf = SrcFormatter.Format(u.AbilityMinRange(i)) + "-" + SrcFormatter.Format(u.AbilityMaxRange(i)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString();
                        }
                        else if (u.AbilityMaxRange(i) == 1)
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "   1";
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "   -";
                        }

                        // 残り使用回数
                        if (withBlock3.Stock > 0)
                        {
                            string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(u.Stock(i)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString1();
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "  -";
                        }

                        // ＥＮ消費量
                        if (withBlock3.ENConsumption > 0)
                        {
                            string localLeftPaddedString2() { string argbuf = SrcFormatter.Format(u.AbilityENConsumption(i)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString2();
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "   -";
                        }

                        // 必要気力
                        if (withBlock3.NecessaryMorale > 0)
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + " 気" + withBlock3.NecessaryMorale;
                        }

                        // 属性
                        string argstring21 = "|";
                        if (GeneralLib.InStrNotNest(ref withBlock3.Class_Renamed, ref argstring21) > 0)
                        {
                            string argstring2 = "|";
                            list[Information.UBound(list)] = list[Information.UBound(list)] + " " + Strings.Left(withBlock3.Class_Renamed, GeneralLib.InStrNotNest(ref withBlock3.Class_Renamed, ref argstring2) - 1);
                        }
                        else
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + " " + withBlock3.Class_Renamed;
                        }

                        if (!string.IsNullOrEmpty(rest_msg))
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            Array.Resize(ref alist, Information.UBound(list) + 1);
                            Array.Resize(ref ListItemFlag, Information.UBound(list) + 1);
                            list[Information.UBound(list)] = Strings.Space(21) + rest_msg;
                            alist[Information.UBound(list)] = i;
                            ListItemFlag[Information.UBound(list)] = !is_available;
                        }
                    }

                    NextLoop:
                    ;
                }
            }

            if (Information.UBound(list) == 0)
            {
                AbilityListBoxRet = 0;
                return AbilityListBoxRet;
            }

            // リストボックスを表示
            TopItem = -1;
            string argtname1 = "EN";
            string arglb_info = "名称                 効果                            射程 数  " + Expression.Term(ref argtname1, ref u, 2) + " 分類";
            string arglb_mode = "表示のみ";
            ret = ListBox(ref caption_msg, ref list, ref arglb_info, ref arglb_mode);
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ダイアログ";
                MoveCursorPos(ref argcursor_mode);
            }

            while (!IsFormClicked)
            {
                Application.DoEvents();
                // 右ボタンでのダブルクリックの実現
                if ((GetAsyncKeyState(RButtonID) & 0x8000) == 0)
                {
                    is_rbutton_released = true;
                }
                else if (is_rbutton_released)
                {
                    IsFormClicked = true;
                }
            }

            if (lb_mode != "一覧")
            {
                My.MyProject.Forms.frmListBox.Hide();
            }

            ListItemComment = new string[1];
            AbilityListBoxRet = alist[Commands.SelectedItem];
            Application.DoEvents();
            return AbilityListBoxRet;
        }

        // 入力時間制限付きのリストボックスを表示
        public static short LIPS(ref string lb_caption, ref string[] list, ref string lb_info, short time_limit)
        {
            short LIPSRet = default;
            short i;

            Load(My.MyProject.Forms.frmListBox);
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                // 表示内容を設定
                withBlock.Text = lb_caption;
                withBlock.labCaption.Text = "  " + lb_info;
                withBlock.lstItems.Items.Clear();
                var loopTo = (short)Information.UBound(list);
                for (i = 1; i <= loopTo; i++)
                    withBlock.lstItems.Items.Add("  " + list[i]);
                withBlock.lstItems.SelectedIndex = 0;
                withBlock.lstItems.Height = 86;

                // 表示位置を設定
                withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
                if (MainForm.Visible == true & !((int)MainForm.WindowState == 1))
                {
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(MainForm.Top) + SrcFormatter.PixelsToTwipsY(MainForm.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height));
                }
                else
                {
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);
                }

                // 入力制限時間に関する設定を行う
                withBlock.CurrentTime = 0;
                withBlock.TimeLimit = time_limit;
                withBlock.picBar.Visible = true;
                withBlock.Timer1.Enabled = true;

                // リストボックスを表示し、プレイヤーからの入力を待つ
                Commands.SelectedItem = 0;
                IsFormClicked = false;
                withBlock.ShowDialog();
                withBlock.CurrentTime = 0;
                LIPSRet = Commands.SelectedItem;

                // リストボックスを消去
                withBlock.lstItems.Height = 100;
                withBlock.picBar.Visible = false;
                withBlock.Timer1.Enabled = false;
            }

            return LIPSRet;
        }

        // 複数段のリストボックスを表示
        public static short MultiColumnListBox(ref string lb_caption, ref string[] list, bool is_center)
        {
            short MultiColumnListBoxRet = default;
            short i;

            Load(My.MyProject.Forms.frmMultiColumnListBox);
            {
                var withBlock = My.MyProject.Forms.frmMultiColumnListBox;
                withBlock.Text = lb_caption;
                withBlock.lstItems.Visible = false;
                withBlock.lstItems.Items.Clear();

                // アイテムを追加
                var loopTo = (short)Information.UBound(list);
                for (i = 1; i <= loopTo; i++)
                {
                    if (ListItemFlag[i])
                    {
                        withBlock.lstItems.Items.Add("×" + list[i]);
                    }
                    else
                    {
                        withBlock.lstItems.Items.Add("  " + list[i]);
                    }
                }

                var loopTo1 = (short)Information.UBound(list);
                for (i = 1; i <= loopTo1; i++)
                {
                    if (!ListItemFlag[Information.UBound(list) - i + 1])
                    {
                        withBlock.lstItems.SelectedIndex = Information.UBound(list) - i;
                        break;
                    }
                }

                withBlock.lstItems.SelectedIndex = -1;
                withBlock.lstItems.Visible = true;
                if (Information.UBound(ListItemComment) != Information.UBound(list))
                {
                    Array.Resize(ref ListItemComment, Information.UBound(list) + 1);
                }

                // 表示位置を設定
                withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
                if (MainForm.Visible == true & !((int)MainForm.WindowState == 1) & !is_center)
                {
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(MainForm.Top) + SrcFormatter.PixelsToTwipsY(MainForm.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height));
                }
                else
                {
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);
                }

                // 先頭に表示するアイテムを設定
                if (TopItem > 0)
                {
                    if (withBlock.lstItems.TopIndex != TopItem - 1)
                    {
                        withBlock.lstItems.TopIndex = GeneralLib.MinLng(TopItem, withBlock.lstItems.Items.Count) - 1;
                    }
                }

                Commands.SelectedItem = 0;
                Application.DoEvents();
                IsFormClicked = false;

                // リストボックスを表示
                IsMordal = false;
                withBlock.Show();
                while (!IsFormClicked)
                    Application.DoEvents();
                My.MyProject.Forms.frmMultiColumnListBox.Close();
                // UPGRADE_NOTE: オブジェクト frmMultiColumnListBox をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                My.MyProject.Forms.frmMultiColumnListBox = null;
                MultiColumnListBoxRet = Commands.SelectedItem;
            }

            return MultiColumnListBoxRet;
        }

        // 複数のアイテム選択可能なリストボックスを表示
        public static short MultiSelectListBox(string lb_caption, ref string[] list, string lb_info, short max_num)
        {
            short MultiSelectListBoxRet = default;
            short i, j;

            // ステータスウィンドウに攻撃の命中率などを表示させないようにする
            Commands.CommandState = "ユニット選択";

            // リストボックスを作成して表示
            Load(My.MyProject.Forms.frmMultiSelectListBox);
            {
                var withBlock = My.MyProject.Forms.frmMultiSelectListBox;
                withBlock.Text = lb_caption;
                withBlock.lblCaption.Text = "　" + lb_info;
                MaxListItem = max_num;
                var loopTo = (short)Information.UBound(list);
                for (i = 1; i <= loopTo; i++)
                    withBlock.lstItems.Items.Add("　" + list[i]);
                withBlock.cmdSort.Text = "名称順に並べ替え";
                withBlock.Left = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX(MainForm.Left));
                withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);
                withBlock.ShowDialog();
            }

            // 選択された項目数を返す
            j = 0;
            var loopTo1 = (short)Information.UBound(list);
            for (i = 1; i <= loopTo1; i++)
            {
                if (ListItemFlag[i])
                {
                    j = (short)(j + 1);
                }
            }

            MultiSelectListBoxRet = j;

            // リストボックスを消去
            My.MyProject.Forms.frmMultiSelectListBox.Close();
            // UPGRADE_NOTE: オブジェクト frmMultiSelectListBox をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            My.MyProject.Forms.frmMultiSelectListBox = null;
            return MultiSelectListBoxRet;
        }


        // === 画像描画に関する処理 ===

        // 画像をウィンドウに描画
        public static bool DrawPicture(ref string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, ref string draw_option)
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
            PictureBox pic, mask_pic = default;
            PictureBox stretched_pic, stretched_mask_pic = default;
            PictureBox orig_pic;
            int orig_width, orig_height;
            bool found_orig = default, in_history, load_only = default;
            var is_colorfilter = default(bool);
            var fcolor = default(int);
            double trans_par;
            string tnum, tdir, tname;
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
                    Static init_draw_pitcure As Boolean

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
                    Static scenario_bitmap_dir_exists As Boolean

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
                    Static extdata_bitmap_dir_exists As Boolean

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
                    Static extdata2_bitmap_dir_exists As Boolean

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
                    Static scenario_anime_bitmap_dir_exists As Boolean

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
                    Static extdata_anime_bitmap_dir_exists As Boolean

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
                    Static extdata2_anime_bitmap_dir_exists As Boolean

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
                    Static scenario_event_bitmap_dir_exists As Boolean

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
                    Static extdata_event_bitmap_dir_exists As Boolean

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
                    Static extdata2_event_bitmap_dir_exists As Boolean

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
                    Static scenario_cutin_bitmap_dir_exists As Boolean

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
                    Static extdata_cutin_bitmap_dir_exists As Boolean

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
                    Static extdata2_cutin_bitmap_dir_exists As Boolean

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
                    Static app_cutin_bitmap_dir_exists As Boolean

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
                    Static scenario_pilot_bitmap_dir_exists As Boolean

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
                    Static extdata_pilot_bitmap_dir_exists As Boolean

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
                    Static extdata2_pilot_bitmap_dir_exists As Boolean

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
                    Static app_pilot_bitmap_dir_exists As Boolean

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
                    Static scenario_unit_bitmap_dir_exists As Boolean

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
                    Static extdata_unit_bitmap_dir_exists As Boolean

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
                    Static extdata2_unit_bitmap_dir_exists As Boolean

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
                    Static app_unit_bitmap_dir_exists As Boolean

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
                    Static scenario_map_bitmap_dir_exists As Boolean

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
                    Static extdata_map_bitmap_dir_exists As Boolean

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
                    Static extdata2_map_bitmap_dir_exists As Boolean

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
                    Static display_byte_pixel As Integer

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
                    Static last_fname As String

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
                    Static last_exists As Boolean

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
                    Static last_path As String

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
                    Static last_angle As Integer

             */
            ;

            // 初回実行時に各種情報の初期化を行う
            if (!init_draw_pitcure)
            {
                // 各フォルダにBitmapフォルダがあるかチェック
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + "Bitmap", FileAttribute.Directory)) > 0)
                {
                    scenario_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + "Bitmap", FileAttribute.Directory)) > 0)
                {
                    extdata_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + "Bitmap", FileAttribute.Directory)) > 0)
                {
                    extdata2_bitmap_dir_exists = true;
                }

                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + @"Bitmap\Anime", FileAttribute.Directory)) > 0)
                {
                    scenario_anime_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + @"Bitmap\Anime", FileAttribute.Directory)) > 0)
                {
                    extdata_anime_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + @"Bitmap\Anime", FileAttribute.Directory)) > 0)
                {
                    extdata2_anime_bitmap_dir_exists = true;
                }

                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + @"Bitmap\Event", FileAttribute.Directory)) > 0)
                {
                    scenario_event_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + @"Bitmap\Event", FileAttribute.Directory)) > 0)
                {
                    extdata_event_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + @"Bitmap\Event", FileAttribute.Directory)) > 0)
                {
                    extdata2_event_bitmap_dir_exists = true;
                }

                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + @"Bitmap\Cutin", FileAttribute.Directory)) > 0)
                {
                    scenario_cutin_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + @"Bitmap\Cutin", FileAttribute.Directory)) > 0)
                {
                    extdata_cutin_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + @"Bitmap\Cutin", FileAttribute.Directory)) > 0)
                {
                    extdata2_cutin_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.AppPath + @"Bitmap\Cutin", FileAttribute.Directory)) > 0)
                {
                    app_cutin_bitmap_dir_exists = true;
                }

                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + @"Bitmap\Pilot", FileAttribute.Directory)) > 0)
                {
                    scenario_pilot_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + @"Bitmap\Pilot", FileAttribute.Directory)) > 0)
                {
                    extdata_pilot_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + @"Bitmap\Pilot", FileAttribute.Directory)) > 0)
                {
                    extdata2_pilot_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.AppPath + @"Bitmap\Pilot", FileAttribute.Directory)) > 0)
                {
                    app_pilot_bitmap_dir_exists = true;
                }

                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + @"Bitmap\Unit", FileAttribute.Directory)) > 0)
                {
                    scenario_unit_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + @"Bitmap\Unit", FileAttribute.Directory)) > 0)
                {
                    extdata_unit_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + @"Bitmap\Unit", FileAttribute.Directory)) > 0)
                {
                    extdata2_unit_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.AppPath + @"Bitmap\Unit", FileAttribute.Directory)) > 0)
                {
                    app_unit_bitmap_dir_exists = true;
                }

                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + @"Bitmap\Map", FileAttribute.Directory)) > 0)
                {
                    scenario_map_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + @"Bitmap\Map", FileAttribute.Directory)) > 0)
                {
                    extdata_map_bitmap_dir_exists = true;
                }
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + @"Bitmap\Map", FileAttribute.Directory)) > 0)
                {
                    extdata2_map_bitmap_dir_exists = true;
                }

                // 画面の色数を参照
                display_byte_pixel = GetDeviceCaps(MainForm.picMain(0).hDC, BITSPIXEL) / 8;
                init_draw_pitcure = true;
            }

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

            // オプションの解析
            BGColor = ColorTranslator.ToOle(Color.White);
            // マスク画像に影響しないオプション
            pic_option = "";
            // マスク画像に影響するオプション
            pic_option2 = "";
            // フィルタ時の透過度を初期化
            trans_par = -1;
            i = 1;
            while (i <= GeneralLib.LLength(ref draw_option))
            {
                opt = GeneralLib.LIndex(ref draw_option, i);
                switch (opt ?? "")
                {
                    case "背景":
                        {
                            permanent = true;
                            // 背景書き込みで夜やセピア色のマップの場合は指定がなくても特殊効果を付ける
                            switch (Map.MapDrawMode ?? "")
                            {
                                case "夜":
                                    {
                                        dark_count = (short)(dark_count + 1);
                                        pic_option = pic_option + " 暗";
                                        break;
                                    }

                                case "白黒":
                                    {
                                        is_monotone = true;
                                        pic_option = pic_option + " 白黒";
                                        break;
                                    }

                                case "セピア":
                                    {
                                        is_sepia = true;
                                        pic_option = pic_option + " セピア";
                                        break;
                                    }

                                case "夕焼け":
                                    {
                                        is_sunset = true;
                                        pic_option = pic_option + " 夕焼け";
                                        break;
                                    }

                                case "水中":
                                    {
                                        is_water = true;
                                        pic_option = pic_option + " 水中";
                                        break;
                                    }

                                case "フィルタ":
                                    {
                                        is_colorfilter = true;
                                        fcolor = Map.MapDrawFilterColor;
                                        pic_option2 = pic_option2 + " フィルタ=" + Map.MapDrawFilterColor.ToString();
                                        break;
                                    }
                            }

                            break;
                        }

                    case "透過":
                        {
                            transparent = true;
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "白黒":
                        {
                            is_monotone = true;
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "セピア":
                        {
                            is_sepia = true;
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "夕焼け":
                        {
                            is_sunset = true;
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "水中":
                        {
                            is_water = true;
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "明":
                        {
                            bright_count = (short)(bright_count + 1);
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "暗":
                        {
                            dark_count = (short)(dark_count + 1);
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "左右反転":
                        {
                            hrev = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "上下反転":
                        {
                            vrev = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "ネガポジ反転":
                        {
                            negpos = true;
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "シルエット":
                        {
                            is_sil = true;
                            pic_option = pic_option + " " + opt;
                            break;
                        }

                    case "上半分":
                        {
                            top_part = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "下半分":
                        {
                            bottom_part = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "右半分":
                        {
                            right_part = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "左半分":
                        {
                            left_part = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "右上":
                        {
                            tright_part = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "左上":
                        {
                            tleft_part = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "右下":
                        {
                            bright_part = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "左下":
                        {
                            bleft_part = true;
                            pic_option2 = pic_option2 + " " + opt;
                            break;
                        }

                    case "メッセージ":
                        {
                            on_msg_window = true;
                            break;
                        }

                    case "ステータス":
                        {
                            on_status_window = true;
                            break;
                        }

                    case "保持":
                        {
                            keep_picture = true;
                            break;
                        }

                    case "右回転":
                        {
                            i = (short)(i + 1);
                            string argexpr = GeneralLib.LIndex(ref draw_option, i);
                            angle = GeneralLib.StrToLng(ref argexpr);
                            pic_option2 = pic_option2 + " 右回転=" + SrcFormatter.Format(angle % 360);
                            break;
                        }

                    case "左回転":
                        {
                            i = (short)(i + 1);
                            int localStrToLng() { string argexpr = GeneralLib.LIndex(ref draw_option, i); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                            angle = -localStrToLng();
                            pic_option2 = pic_option2 + " 右回転=" + SrcFormatter.Format(angle % 360);
                            break;
                        }

                    case "フィルタ":
                        {
                            is_colorfilter = true;
                            break;
                        }

                    default:
                        {
                            if (Strings.Right(opt, 1) == "%" & Information.IsNumeric(Strings.Left(opt, Strings.Len(opt) - 1)))
                            {
                                trans_par = GeneralLib.MaxDbl(0d, GeneralLib.MinDbl(1d, Conversions.ToDouble(Strings.Left(opt, Strings.Len(opt) - 1)) / 100d));
                                pic_option2 = pic_option2 + " フィルタ透過度=" + opt;
                            }
                            else if (is_colorfilter)
                            {
                                fcolor = Conversions.ToInteger(opt);
                                pic_option2 = pic_option2 + " フィルタ=" + opt;
                            }
                            else
                            {
                                BGColor = Conversions.ToInteger(opt);
                                pic_option2 = pic_option2 + " " + opt;
                            }

                            break;
                        }
                }

                i = (short)(i + 1);
            }

            pic_option = Strings.Trim(pic_option);
            pic_option2 = Strings.Trim(pic_option2);

            // 描画先を設定
            if (on_msg_window)
            {
                // メッセージウィンドウへのパイロット画像の描画
                pic = My.MyProject.Forms.frmMessage.picFace;
                permanent = false;
            }
            else if (on_status_window)
            {
                // ステータスウィンドウへのパイロット画像の描画
                pic = MainForm.picUnitStatus;
            }
            else if (permanent)
            {
                // 背景への描画
                pic = MainForm.picBack;
            }
            else
            {
                // マップウィンドウへの通常の描画
                pic = MainForm.picMain(0);
                SaveScreen();
            }

            // 読み込むファイルの探索

            // 前回の画像ファイルと同じ？
            if ((fname ?? "") == (last_fname ?? ""))
            {
                // 前回ファイルは見つかっていたのか？
                if (!last_exists)
                {
                    DrawPictureRet = false;
                    return DrawPictureRet;
                }
            }

            // 以前表示した拡大画像が利用可能？
            var loopTo = (short)(SRC.ImageBufferSize - 1);
            for (i = 0; i <= loopTo; i++)
            {
                // 同じファイル？
                if ((PicBufFname[i] ?? "") == (fname ?? ""))
                {
                    // オプションも同じ？
                    if ((PicBufOption[i] ?? "") == (pic_option ?? "") & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & !PicBufIsMask[i] & PicBufDW[i] == dw & PicBufDH[i] == dh & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                    {
                        // 同じファイル、オプションによる画像が見つかった

                        // 以前表示した画像をそのまま利用
                        UsePicBuf(i);
                        orig_pic = MainForm.picBuf(i);
                        {
                            var withBlock = orig_pic;
                            orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock.Width);
                            orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock.Height);
                        }
                        // Debug.Print "Reuse " & Format$(i) & " As Stretched"
                        goto EditedPicture;
                    }
                }
            }

            // 以前表示した画像が利用可能？
            var loopTo1 = (short)(SRC.ImageBufferSize - 1);
            for (i = 0; i <= loopTo1; i++)
            {
                // 同じファイル？
                if ((PicBufFname[i] ?? "") == (fname ?? ""))
                {
                    // オプションも同じ？
                    if ((PicBufOption[i] ?? "") == (pic_option ?? "") & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & !PicBufIsMask[i] & PicBufDW[i] == SRC.DEFAULT_LEVEL & PicBufDH[i] == SRC.DEFAULT_LEVEL & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                    {
                        // 同じファイル、オプションによる画像が見つかった

                        // 以前表示した画像をそのまま利用
                        UsePicBuf(i);
                        orig_pic = MainForm.picBuf(i);
                        {
                            var withBlock1 = orig_pic;
                            orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock1.Width);
                            orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock1.Height);
                        }
                        // Debug.Print "Reuse " & Format$(i) & " As Edited"
                        found_orig = true;
                        goto EditedPicture;
                    }
                }
            }

            // 以前使用した部分画像が利用可能？
            if (sw != 0)
            {
                var loopTo2 = (short)(SRC.ImageBufferSize - 1);
                for (i = 0; i <= loopTo2; i++)
                {
                    // 同じファイル？
                    if ((PicBufFname[i] ?? "") == (fname ?? ""))
                    {
                        if (string.IsNullOrEmpty(PicBufOption[i]) & string.IsNullOrEmpty(PicBufOption2[i]) & !PicBufIsMask[i] & PicBufDW[i] == SRC.DEFAULT_LEVEL & PicBufDH[i] == SRC.DEFAULT_LEVEL & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                        {
                            // 以前使用した部分画像をそのまま利用
                            UsePicBuf(i);
                            orig_pic = MainForm.picBuf(i);
                            {
                                var withBlock2 = orig_pic;
                                orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock2.Width);
                                orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock2.Height);
                            }
                            // Debug.Print "Reuse " & Format$(i) & " As Partial"
                            goto LoadedOrigPicture;
                        }
                    }
                }
            }

            // 以前使用した原画像が利用可能？
            var loopTo3 = (short)(SRC.ImageBufferSize - 1);
            for (i = 0; i <= loopTo3; i++)
            {
                // 同じファイル？
                if ((PicBufFname[i] ?? "") == (fname ?? ""))
                {
                    if (string.IsNullOrEmpty(PicBufOption[i]) & string.IsNullOrEmpty(PicBufOption2[i]) & !PicBufIsMask[i] & PicBufDW[i] == SRC.DEFAULT_LEVEL & PicBufDH[i] == SRC.DEFAULT_LEVEL & PicBufSW[i] == 0)
                    {
                        // 以前使用した原画像をそのまま利用
                        UsePicBuf(i);
                        orig_pic = MainForm.picBuf(i);
                        {
                            var withBlock3 = orig_pic;
                            orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock3.Width);
                            orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock3.Height);
                        }
                        // Debug.Print "Reuse " & Format$(i) & " As Orig"
                        goto LoadedOrigPicture;
                    }
                }
            }

            // 特殊なファイル名
            switch (Strings.LCase(fname) ?? "")
            {
                case "black.bmp":
                case @"event\black.bmp":
                    {
                        // 黒で塗りつぶし
                        if (dx == SRC.DEFAULT_LEVEL)
                        {
                            dx = (int)((long)(SrcFormatter.PixelsToTwipsX(pic.Width) - dw) / 2L);
                        }

                        if (dy == SRC.DEFAULT_LEVEL)
                        {
                            dy = (int)((long)(SrcFormatter.PixelsToTwipsY(pic.Height) - dh) / 2L);
                        }
                        ret = PatBlt(pic.hDC, dx, dy, dw, dh, BLACKNESS);
                        goto DrewPicture;
                        break;
                    }

                case "white.bmp":
                case @"event\white.bmp":
                    {
                        // 白で塗りつぶし
                        if (dx == SRC.DEFAULT_LEVEL)
                        {
                            dx = (int)((long)(SrcFormatter.PixelsToTwipsX(pic.Width) - dw) / 2L);
                        }

                        if (dy == SRC.DEFAULT_LEVEL)
                        {
                            dy = (int)((long)(SrcFormatter.PixelsToTwipsY(pic.Height) - dh) / 2L);
                        }
                        ret = PatBlt(pic.hDC, dx, dy, dw, dh, WHITENESS);
                        goto DrewPicture;
                        break;
                    }

                case @"common\effect_tile(ally).bmp":
                case @"anime\common\effect_tile(ally).bmp":
                    {
                        // 味方ユニットタイル
                        orig_pic = MainForm.picUnit;
                        orig_width = 32;
                        orig_height = 32;
                        goto LoadedOrigPicture;
                        break;
                    }

                case @"common\effect_tile(enemy).bmp":
                case @"anime\common\effect_tile(enemy).bmp":
                    {
                        // 敵ユニットタイル
                        orig_pic = MainForm.picEnemy;
                        orig_width = 32;
                        orig_height = 32;
                        goto LoadedOrigPicture;
                        break;
                    }

                case @"common\effect_tile(neutral).bmp":
                case @"anime\common\effect_tile(neutral).bmp":
                    {
                        // 中立ユニットタイル
                        orig_pic = MainForm.picNeautral;
                        orig_width = 32;
                        orig_height = 32;
                        goto LoadedOrigPicture;
                        break;
                    }
            }

            // フルパスで指定されている？
            if (Strings.InStr(fname, ":") == 2)
            {
                fpath = "";
                last_path = "";
                // 登録を避けるため
                in_history = true;
                goto FoundPicture;

                // 履歴を検索してみる
            };
            // UPGRADE_WARNING: オブジェクト fpath_history.Item() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            fpath = Conversions.ToString(fpath_history[fname]);

            // 履歴上にファイルを発見
            last_path = "";
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToZeroStatement not implemented, please report this issue in 'On Error GoTo 0' at character 333274


            Input:

                    '履歴上にファイルを発見
                    On Error GoTo 0

             */
            if (string.IsNullOrEmpty(fpath))
            {
                // ファイルは存在しない
                last_fname = fname;
                last_exists = false;
                DrawPictureRet = false;
                return DrawPictureRet;
            }

            in_history = true;
            goto FoundPicture;


            // 履歴になかった
            NotFound:
            ;
            ;

            // 戦闘アニメ用？
            if (Strings.InStr(fname, @"\EFFECT_") > 0)
            {
                if (scenario_anime_bitmap_dir_exists)
                {
                    string argfname = SRC.ScenarioPath + @"Bitmap\Anime\" + fname;
                    if (GeneralLib.FileExists(ref argfname))
                    {
                        fpath = SRC.ScenarioPath + @"Bitmap\Anime\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata_anime_bitmap_dir_exists)
                {
                    string argfname1 = SRC.ExtDataPath + @"Bitmap\Anime\" + fname;
                    if (GeneralLib.FileExists(ref argfname1))
                    {
                        fpath = SRC.ExtDataPath + @"Bitmap\Anime\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata2_anime_bitmap_dir_exists)
                {
                    string argfname2 = SRC.ExtDataPath2 + @"Bitmap\Anime\" + fname;
                    if (GeneralLib.FileExists(ref argfname2))
                    {
                        fpath = SRC.ExtDataPath2 + @"Bitmap\Anime\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                string argfname3 = SRC.AppPath + @"Bitmap\Anime\" + fname;
                if (GeneralLib.FileExists(ref argfname3))
                {
                    fpath = SRC.AppPath + @"Bitmap\Anime\";
                    last_path = "";
                    goto FoundPicture;
                }
            }

            // 前回と同じパス？
            if (Strings.Len(last_path) > 0)
            {
                string argfname4 = last_path + fname;
                if (GeneralLib.FileExists(ref argfname4))
                {
                    fpath = last_path;
                    goto FoundPicture;
                }
            }

            // パス名入り？
            if (Strings.InStr(fname, @"Bitmap\") > 0)
            {
                if (scenario_bitmap_dir_exists)
                {
                    string argfname5 = SRC.ScenarioPath + fname;
                    if (GeneralLib.FileExists(ref argfname5))
                    {
                        fpath = SRC.ScenarioPath;
                        last_path = fpath;
                        goto FoundPicture;
                    }
                }

                string argfname6 = SRC.AppPath + fname;
                if (GeneralLib.FileExists(ref argfname6))
                {
                    fpath = SRC.AppPath;
                    last_path = "";
                    goto FoundPicture;
                }

                if (Strings.Mid(fname, 2, 1) == ":")
                {
                    fpath = "";
                    last_path = "";
                    goto FoundPicture;
                }
            }

            // フォルダ指定あり？
            if (Strings.InStr(fname, @"\") > 0)
            {
                if (scenario_bitmap_dir_exists)
                {
                    string argfname7 = SRC.ScenarioPath + @"Bitmap\" + fname;
                    if (GeneralLib.FileExists(ref argfname7))
                    {
                        fpath = SRC.ScenarioPath + @"Bitmap\";
                        last_path = fpath;
                        goto FoundPicture;
                    }
                }

                if (extdata_bitmap_dir_exists)
                {
                    string argfname8 = SRC.ExtDataPath + @"Bitmap\" + fname;
                    if (GeneralLib.FileExists(ref argfname8))
                    {
                        fpath = SRC.ExtDataPath + @"Bitmap\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata2_bitmap_dir_exists)
                {
                    string argfname9 = SRC.ExtDataPath2 + @"Bitmap\" + fname;
                    if (GeneralLib.FileExists(ref argfname9))
                    {
                        fpath = SRC.ExtDataPath2 + @"Bitmap\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                string argfname10 = SRC.AppPath + @"Bitmap\" + fname;
                if (GeneralLib.FileExists(ref argfname10))
                {
                    fpath = SRC.AppPath + @"Bitmap\";
                    last_path = "";
                    goto FoundPicture;
                }

                if (Strings.LCase(Strings.Left(fname, 4)) == @"map\")
                {
                    tname = Strings.Mid(fname, 5);
                    if (Strings.InStr(tname, @"\") == 0)
                    {
                        i = (short)(Strings.Len(tname) - 5);
                        while (i > 0)
                        {
                            if (LikeOperator.LikeString(Strings.Mid(tname, i, 1), "[!-0-9]", CompareMethod.Binary))
                            {
                                break;
                            }

                            i = (short)(i - 1);
                        }

                        if (i > 0)
                        {
                            tdir = Strings.Left(tname, i) + @"\";
                            tnum = Strings.Mid(tname, i + 1, Strings.Len(tname) - i - 4);
                            tname = Strings.Left(tname, i) + SrcFormatter.Format(GeneralLib.StrToLng(ref tnum), "0000") + ".bmp";
                        }
                    }
                }
            }
            // 地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
            else if (LikeOperator.LikeString(fname, "*#.bmp", CompareMethod.Binary) & LikeOperator.LikeString(Strings.Left(fname, 1), "[a-z]", CompareMethod.Binary))
            {
                i = (short)(Strings.Len(fname) - 5);
                while (i > 0)
                {
                    if (LikeOperator.LikeString(Strings.Mid(fname, i, 1), "[!-0-9]", CompareMethod.Binary))
                    {
                        break;
                    }

                    i = (short)(i - 1);
                }

                if (i > 0)
                {
                    tdir = Strings.Left(fname, i);
                    {
                        var withBlock4 = SRC.TDList;
                        var loopTo4 = withBlock4.Count;
                        for (j = 1; j <= loopTo4; j++)
                        {
                            if (tdir == withBlock4.Item(withBlock4.OrderedID(j)).Bitmap)
                            {
                                tnum = Strings.Mid(fname, i + 1, Strings.Len(fname) - i - 4);
                                tname = Strings.Left(fname, i) + SrcFormatter.Format(GeneralLib.StrToLng(ref tnum), "0000") + ".bmp";
                                break;
                            }
                        }

                        if (j <= withBlock4.Count)
                        {
                            tdir = tdir + @"\";
                        }
                        else
                        {
                            tdir = "";
                        }
                    }
                }
            }

            // 各フォルダを検索する

            // Bitmapフォルダに直置き
            if (scenario_map_bitmap_dir_exists)
            {
                string argfname11 = SRC.ScenarioPath + @"Bitmap\" + fname;
                if (GeneralLib.FileExists(ref argfname11))
                {
                    fpath = SRC.ScenarioPath + @"Bitmap\";
                    last_path = fpath;
                    goto FoundPicture;
                }
            }

            string argfname12 = SRC.ScenarioPath + @"Bitmap\" + fname;
            if (GeneralLib.FileExists(ref argfname12))
            {
                fpath = SRC.ScenarioPath + @"Bitmap\";
                last_path = fpath;
                goto FoundPicture;
            }

            // シナリオフォルダ
            if (scenario_bitmap_dir_exists)
            {
                if (scenario_anime_bitmap_dir_exists)
                {
                    string argfname13 = SRC.ScenarioPath + @"Bitmap\Anime\" + fname;
                    if (GeneralLib.FileExists(ref argfname13))
                    {
                        fpath = SRC.ScenarioPath + @"Bitmap\Anime\";
                        last_path = fpath;
                        goto FoundPicture;
                    }
                }

                if (scenario_event_bitmap_dir_exists)
                {
                    string argfname14 = SRC.ScenarioPath + @"Bitmap\Event\" + fname;
                    if (GeneralLib.FileExists(ref argfname14))
                    {
                        fpath = SRC.ScenarioPath + @"Bitmap\Event\";
                        last_path = fpath;
                        goto FoundPicture;
                    }
                }

                if (scenario_cutin_bitmap_dir_exists)
                {
                    string argfname15 = SRC.ScenarioPath + @"Bitmap\Cutin\" + fname;
                    if (GeneralLib.FileExists(ref argfname15))
                    {
                        fpath = SRC.ScenarioPath + @"Bitmap\Cutin\";
                        last_path = fpath;
                        goto FoundPicture;
                    }
                }

                if (scenario_pilot_bitmap_dir_exists)
                {
                    string argfname16 = SRC.ScenarioPath + @"Bitmap\Pilot\" + fname;
                    if (GeneralLib.FileExists(ref argfname16))
                    {
                        fpath = SRC.ScenarioPath + @"Bitmap\Pilot\";
                        last_path = fpath;
                        goto FoundPicture;
                    }
                }

                if (scenario_unit_bitmap_dir_exists)
                {
                    string argfname17 = SRC.ScenarioPath + @"Bitmap\Unit\" + fname;
                    if (GeneralLib.FileExists(ref argfname17))
                    {
                        fpath = SRC.ScenarioPath + @"Bitmap\Unit\";
                        last_path = fpath;
                        goto FoundPicture;
                    }
                }

                if (scenario_map_bitmap_dir_exists)
                {
                    if (!string.IsNullOrEmpty(tdir))
                    {
                        string argfname18 = SRC.ScenarioPath + @"Bitmap\Map\" + tdir + fname;
                        if (GeneralLib.FileExists(ref argfname18))
                        {
                            fpath = SRC.ScenarioPath + @"Bitmap\Map\" + tdir;
                            last_path = fpath;
                            goto FoundPicture;
                        }

                        string argfname19 = SRC.ScenarioPath + @"Bitmap\Map\" + tdir + tname;
                        if (GeneralLib.FileExists(ref argfname19))
                        {
                            fname = tname;
                            fpath = SRC.ScenarioPath + @"Bitmap\Map\" + tdir;
                            last_path = fpath;
                            // 登録を避けるため
                            in_history = true;
                            goto FoundPicture;
                        }

                        string argfname20 = SRC.ScenarioPath + @"Bitmap\Map\" + tname;
                        if (GeneralLib.FileExists(ref argfname20))
                        {
                            fname = tname;
                            fpath = SRC.ScenarioPath + @"Bitmap\Map\";
                            last_path = fpath;
                            // 登録を避けるため
                            in_history = true;
                            goto FoundPicture;
                        }
                    }

                    string argfname21 = SRC.ScenarioPath + @"Bitmap\Map\" + fname;
                    if (GeneralLib.FileExists(ref argfname21))
                    {
                        fpath = SRC.ScenarioPath + @"Bitmap\Map\";
                        last_path = fpath;
                        goto FoundPicture;
                    }
                }
            }

            // ExtDataPath
            if (extdata_bitmap_dir_exists)
            {
                if (extdata_anime_bitmap_dir_exists)
                {
                    string argfname22 = SRC.ExtDataPath + @"Bitmap\Anime\" + fname;
                    if (GeneralLib.FileExists(ref argfname22))
                    {
                        fpath = SRC.ExtDataPath + @"Bitmap\Anime\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata_event_bitmap_dir_exists)
                {
                    string argfname23 = SRC.ExtDataPath + @"Bitmap\Event\" + fname;
                    if (GeneralLib.FileExists(ref argfname23))
                    {
                        fpath = SRC.ExtDataPath + @"Bitmap\Event\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata_cutin_bitmap_dir_exists)
                {
                    string argfname24 = SRC.ExtDataPath + @"Bitmap\Cutin\" + fname;
                    if (GeneralLib.FileExists(ref argfname24))
                    {
                        fpath = SRC.ExtDataPath + @"Bitmap\Cutin\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata_pilot_bitmap_dir_exists)
                {
                    string argfname25 = SRC.ExtDataPath + @"Bitmap\Pilot\" + fname;
                    if (GeneralLib.FileExists(ref argfname25))
                    {
                        fpath = SRC.ExtDataPath + @"Bitmap\Pilot\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata_unit_bitmap_dir_exists)
                {
                    string argfname26 = SRC.ExtDataPath + @"Bitmap\Unit\" + fname;
                    if (GeneralLib.FileExists(ref argfname26))
                    {
                        fpath = SRC.ExtDataPath + @"Bitmap\Unit\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata_map_bitmap_dir_exists)
                {
                    if (!string.IsNullOrEmpty(tdir))
                    {
                        string argfname27 = SRC.ExtDataPath + @"Bitmap\Map\" + tdir + fname;
                        if (GeneralLib.FileExists(ref argfname27))
                        {
                            fpath = SRC.ExtDataPath + @"Bitmap\Map\" + tdir;
                            last_path = "";
                            goto FoundPicture;
                        }

                        string argfname28 = SRC.ExtDataPath + @"Bitmap\Map\" + tdir + tname;
                        if (GeneralLib.FileExists(ref argfname28))
                        {
                            fname = tname;
                            fpath = SRC.ExtDataPath + @"Bitmap\Map\" + tdir;
                            last_path = "";
                            // 登録を避けるため
                            in_history = true;
                            goto FoundPicture;
                        }

                        string argfname29 = SRC.ExtDataPath + @"Bitmap\Map\" + tname;
                        if (GeneralLib.FileExists(ref argfname29))
                        {
                            fname = tname;
                            fpath = SRC.ExtDataPath + @"Bitmap\Map\";
                            last_path = "";
                            // 登録を避けるため
                            in_history = true;
                            goto FoundPicture;
                        }
                    }

                    string argfname30 = SRC.ExtDataPath + @"Bitmap\Map\" + fname;
                    if (GeneralLib.FileExists(ref argfname30))
                    {
                        fpath = SRC.ExtDataPath + @"Bitmap\Map\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }
            }

            // ExtDataPath2
            if (extdata2_bitmap_dir_exists)
            {
                if (extdata2_anime_bitmap_dir_exists)
                {
                    string argfname31 = SRC.ExtDataPath2 + @"Bitmap\Anime\" + fname;
                    if (GeneralLib.FileExists(ref argfname31))
                    {
                        fpath = SRC.ExtDataPath2 + @"Bitmap\Anime\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata2_event_bitmap_dir_exists)
                {
                    string argfname32 = SRC.ExtDataPath2 + @"Bitmap\Event\" + fname;
                    if (GeneralLib.FileExists(ref argfname32))
                    {
                        fpath = SRC.ExtDataPath2 + @"Bitmap\Event\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata2_cutin_bitmap_dir_exists)
                {
                    string argfname33 = SRC.ExtDataPath2 + @"Bitmap\Cutin\" + fname;
                    if (GeneralLib.FileExists(ref argfname33))
                    {
                        fpath = SRC.ExtDataPath2 + @"Bitmap\Cutin\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata2_pilot_bitmap_dir_exists)
                {
                    string argfname34 = SRC.ExtDataPath2 + @"Bitmap\Pilot\" + fname;
                    if (GeneralLib.FileExists(ref argfname34))
                    {
                        fpath = SRC.ExtDataPath2 + @"Bitmap\Pilot\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata2_unit_bitmap_dir_exists)
                {
                    string argfname35 = SRC.ExtDataPath2 + @"Bitmap\Unit\" + fname;
                    if (GeneralLib.FileExists(ref argfname35))
                    {
                        fpath = SRC.ExtDataPath2 + @"Bitmap\Unit\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }

                if (extdata2_map_bitmap_dir_exists)
                {
                    if (!string.IsNullOrEmpty(tdir))
                    {
                        string argfname36 = SRC.ExtDataPath2 + @"Bitmap\Map\" + tdir + fname;
                        if (GeneralLib.FileExists(ref argfname36))
                        {
                            fpath = SRC.ExtDataPath2 + @"Bitmap\Map\" + tdir;
                            last_path = "";
                            goto FoundPicture;
                        }

                        string argfname37 = SRC.ExtDataPath2 + @"Bitmap\Map\" + tdir + tname;
                        if (GeneralLib.FileExists(ref argfname37))
                        {
                            fname = tname;
                            fpath = SRC.ExtDataPath2 + @"Bitmap\Map\" + tdir;
                            last_path = "";
                            // 登録を避けるため
                            in_history = true;
                            goto FoundPicture;
                        }

                        string argfname38 = SRC.ExtDataPath2 + @"Bitmap\Map\" + tname;
                        if (GeneralLib.FileExists(ref argfname38))
                        {
                            fname = tname;
                            fpath = SRC.ExtDataPath2 + @"Bitmap\Map\";
                            last_path = "";
                            // 登録を避けるため
                            in_history = true;
                            goto FoundPicture;
                        }
                    }

                    string argfname39 = SRC.ExtDataPath2 + @"Bitmap\Map\" + fname;
                    if (GeneralLib.FileExists(ref argfname39))
                    {
                        fpath = SRC.ExtDataPath2 + @"Bitmap\Map\";
                        last_path = "";
                        goto FoundPicture;
                    }
                }
            }

            // 本体側フォルダ
            string argfname40 = SRC.AppPath + @"Bitmap\Anime\" + fname;
            if (GeneralLib.FileExists(ref argfname40))
            {
                fpath = SRC.AppPath + @"Bitmap\Anime\";
                last_path = "";
                goto FoundPicture;
            }

            string argfname41 = SRC.AppPath + @"Bitmap\Event\" + fname;
            if (GeneralLib.FileExists(ref argfname41))
            {
                fpath = SRC.AppPath + @"Bitmap\Event\";
                last_path = "";
                goto FoundPicture;
            }

            if (app_cutin_bitmap_dir_exists)
            {
                string argfname42 = SRC.AppPath + @"Bitmap\Cutin\" + fname;
                if (GeneralLib.FileExists(ref argfname42))
                {
                    fpath = SRC.AppPath + @"Bitmap\Cutin\";
                    last_path = "";
                    goto FoundPicture;
                }
            }

            if (app_pilot_bitmap_dir_exists)
            {
                string argfname43 = SRC.AppPath + @"Bitmap\Pilot\" + fname;
                if (GeneralLib.FileExists(ref argfname43))
                {
                    fpath = SRC.AppPath + @"Bitmap\Pilot\";
                    last_path = "";
                    goto FoundPicture;
                }
            }

            if (app_unit_bitmap_dir_exists)
            {
                string argfname44 = SRC.AppPath + @"Bitmap\Unit\" + fname;
                if (GeneralLib.FileExists(ref argfname44))
                {
                    fpath = SRC.AppPath + @"Bitmap\Unit\";
                    last_path = "";
                    goto FoundPicture;
                }
            }

            if (!string.IsNullOrEmpty(tdir))
            {
                string argfname45 = SRC.AppPath + @"Bitmap\Map\" + tdir + fname;
                if (GeneralLib.FileExists(ref argfname45))
                {
                    fpath = SRC.AppPath + @"Bitmap\Map\" + tdir;
                    last_path = "";
                    goto FoundPicture;
                }

                string argfname46 = SRC.AppPath + @"Bitmap\Map\" + tdir + tname;
                if (GeneralLib.FileExists(ref argfname46))
                {
                    fname = tname;
                    fpath = SRC.AppPath + @"Bitmap\Map\" + tdir;
                    last_path = "";
                    // 登録を避けるため
                    in_history = true;
                    goto FoundPicture;
                }

                string argfname47 = SRC.AppPath + @"Bitmap\Map\" + tname;
                if (GeneralLib.FileExists(ref argfname47))
                {
                    fname = tname;
                    fpath = SRC.AppPath + @"Bitmap\Map\";
                    last_path = "";
                    // 登録を避けるため
                    in_history = true;
                    goto FoundPicture;
                }
            }

            string argfname48 = SRC.AppPath + @"Bitmap\Map\" + fname;
            if (GeneralLib.FileExists(ref argfname48))
            {
                fpath = SRC.AppPath + @"Bitmap\Map\";
                last_path = "";
                goto FoundPicture;
            }

            // 見つからなかった……

            // 履歴に記録しておく
            fpath_history.Add("", fname);

            // 表示を中止
            last_fname = fname;
            last_exists = false;
            DrawPictureRet = false;
            return DrawPictureRet;
            FoundPicture:
            ;


            // ファイル名を記録しておく
            last_fname = fname;

            // 履歴に記録しておく
            if (!in_history)
            {
                fpath_history.Add(fpath, fname);
            }

            last_exists = true;
            pfname = fpath + fname;

            // 使用するバッファを選択
            i = GetPicBuf();
            orig_pic = MainForm.picBuf(i);
            PicBufFname[i] = fname;
            PicBufOption[i] = "";
            PicBufOption2[i] = "";
            PicBufDW[i] = SRC.DEFAULT_LEVEL;
            PicBufDH[i] = SRC.DEFAULT_LEVEL;
            PicBufSX[i] = 0;
            PicBufSY[i] = 0;
            PicBufSW[i] = 0;
            PicBufSH[i] = 0;
            PicBufIsMask[i] = false;
            // Debug.Print "Use " & Format$(i) & " As Orig"

            Susie.LoadPicture2(ref orig_pic, ref pfname);

            // 読み込んだ画像のサイズ(バイト数)をバッファ情報に記録しておく
            {
                var withBlock5 = orig_pic;
                PicBufSize[i] = (int)((double)display_byte_pixel * SrcFormatter.PixelsToTwipsX(withBlock5.Width) * SrcFormatter.PixelsToTwipsY(withBlock5.Height));
            }

            LoadedOrigPicture:
            ;
            {
                var withBlock6 = orig_pic;
                orig_width = (int)SrcFormatter.PixelsToTwipsX(withBlock6.Width);
                orig_height = (int)SrcFormatter.PixelsToTwipsY(withBlock6.Height);
            }

            // 原画像の一部のみを描画？
            if (sw != 0)
            {
                if (sw != orig_width | sh != orig_height)
                {
                    // 使用するpicBufを選択
                    i = GUI.GetPicBuf(display_byte_pixel * sw * sh);
                    PicBufFname[i] = fname;
                    PicBufOption[i] = "";
                    PicBufOption2[i] = "";
                    PicBufDW[i] = SRC.DEFAULT_LEVEL;
                    PicBufDH[i] = SRC.DEFAULT_LEVEL;
                    PicBufSX[i] = (short)sx;
                    PicBufSY[i] = (short)sy;
                    PicBufSW[i] = (short)sw;
                    PicBufSH[i] = (short)sh;
                    PicBufIsMask[i] = false;
                    // Debug.Print "Use " & Format$(i) & " As Partial"

                    // 原画像から描画部分をコピー
                    {
                        var withBlock7 = MainForm.picBuf(i);
                        withBlock7.Picture = Image.FromFile("");
                        withBlock7.width = sw;
                        withBlock7.Height = sh;
                        if (sx == SRC.DEFAULT_LEVEL)
                        {
                            sx = (orig_width - sw) / 2;
                        }

                        if (sy == SRC.DEFAULT_LEVEL)
                        {
                            sy = (orig_height - sh) / 2;
                        }
                        ret = BitBlt(withBlock7.hDC, 0, 0, sw, sh, orig_pic.hDC, sx, sy, SRCCOPY);
                    }

                    orig_pic = MainForm.picBuf(i);
                    orig_width = sw;
                    orig_height = sh;
                }
            }

            LoadedPicture:
            ;


            // 原画像を修正して使う場合は原画像を別のpicBufにコピーして修正する
            if (top_part | bottom_part | left_part | right_part | tleft_part | tright_part | bleft_part | bright_part | is_monotone | is_sepia | is_sunset | is_water | negpos | is_sil | vrev | hrev | bright_count > 0 | dark_count > 0 | angle % 360 != 0 | is_colorfilter)
            {
                // 使用するpicBufを選択
                i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                PicBufFname[i] = fname;
                PicBufOption[i] = pic_option;
                PicBufOption2[i] = pic_option2;
                PicBufDW[i] = SRC.DEFAULT_LEVEL;
                PicBufDH[i] = SRC.DEFAULT_LEVEL;
                PicBufSX[i] = (short)sx;
                PicBufSY[i] = (short)sy;
                PicBufSW[i] = (short)sw;
                PicBufSH[i] = (short)sh;
                PicBufIsMask[i] = false;
                // Debug.Print "Use " & Format$(i) & " As Edited"

                // 画像をコピー
                {
                    var withBlock8 = MainForm.picBuf(i);
                    withBlock8.Picture = Image.FromFile("");
                    withBlock8.width = orig_width;
                    withBlock8.Height = orig_height;
                    ret = BitBlt(withBlock8.hDC, 0, 0, orig_width, orig_height, orig_pic.hDC, 0, 0, SRCCOPY);
                }
                orig_pic = MainForm.picBuf(i);
            }

            // 画像の一部を塗りつぶして描画する場合
            if (top_part)
            {
                // 上半分
                orig_pic.Line(0, orig_height / 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            }

            if (bottom_part)
            {
                // 下半分
                orig_pic.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            }

            if (left_part)
            {
                // 左半分
                orig_pic.Line(orig_width / 2, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            }

            if (right_part)
            {
                // 右半分
                orig_pic.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            }

            if (tleft_part)
            {
                // 左上
                var loopTo5 = (short)(orig_width - 1);
                for (i = 0; i <= loopTo5; i++)
                    orig_pic.Line(i, orig_height - 1 - i); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            }

            if (tright_part)
            {
                // 右上
                var loopTo6 = (short)(orig_width - 1);
                for (i = 0; i <= loopTo6; i++)
                    orig_pic.Line(i, i); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            }

            if (bleft_part)
            {
                // 左下
                var loopTo7 = (short)(orig_width - 1);
                for (i = 0; i <= loopTo7; i++)
                    orig_pic.Line(i, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            }

            if (bright_part)
            {
                // 右下
                var loopTo8 = (short)(orig_width - 1);
                for (i = 0; i <= loopTo8; i++)
                    orig_pic.Line(i, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            }

            // 特殊効果
            if (is_monotone | is_sepia | is_sunset | is_water | is_colorfilter | bright_count > 0 | dark_count > 0 | negpos | is_sil | vrev | hrev | angle != 0)
            {
                // 画像のサイズをチェック
                if (orig_width * orig_height % 4 != 0)
                {
                    string argmsg = fname + "の画像サイズが4の倍数になっていません";
                    ErrorMessage(ref argmsg);
                    return DrawPictureRet;
                }

                // イメージをバッファに取り込み
                Graphics.GetImage(ref orig_pic);

                // 白黒
                if (is_monotone)
                {
                    Graphics.Monotone(transparent);
                }

                // セピア
                if (is_sepia)
                {
                    Graphics.Sepia(transparent);
                }

                // 夕焼け
                if (is_sunset)
                {
                    Graphics.Sunset(transparent);
                }

                // 水中
                if (is_water)
                {
                    Graphics.Water(transparent);
                }

                // シルエット
                if (is_sil)
                {
                    Graphics.Silhouette();
                }

                // ネガポジ反転
                if (negpos)
                {
                    Graphics.NegPosReverse(transparent);
                }

                // フィルタ
                if (is_colorfilter)
                {
                    if (trans_par < 0d)
                    {
                        trans_par = 0.5d;
                    }

                    Graphics.ColorFilter(ref fcolor, ref trans_par, transparent);
                }

                // 明 (多段指定可能)
                var loopTo9 = bright_count;
                for (i = 1; i <= loopTo9; i++)
                    Graphics.Bright(transparent);

                // 暗 (多段指定可能)
                var loopTo10 = dark_count;
                for (i = 1; i <= loopTo10; i++)
                    Graphics.Dark(transparent);

                // 左右反転
                if (vrev)
                {
                    Graphics.VReverse();
                }

                // 上下反転
                if (hrev)
                {
                    Graphics.HReverse();
                }

                // 回転
                if (angle != 0)
                {
                    // 前回の回転角が90度の倍数かどうかで描画の際の最適化使用可否を決める
                    // (連続で回転させる場合に描画速度を一定にするため)
                    Graphics.Rotate(angle, last_angle % 90 != 0);
                }

                // 変更した内容をイメージに変換
                Graphics.SetImage(ref orig_pic);

                // バッファを破棄
                Graphics.ClearImage();
            }

            last_angle = angle;
            EditedPicture:
            ;


            // クリッピング処理
            if (dw == SRC.DEFAULT_LEVEL)
            {
                dw = orig_width;
            }

            if (dh == SRC.DEFAULT_LEVEL)
            {
                dh = orig_height;
            }

            if (permanent)
            {
                // 背景描画の場合、センタリングはマップ中央に
                if (dx == SRC.DEFAULT_LEVEL)
                {
                    dx = (MapPWidth - dw) / 2;
                }

                if (dy == SRC.DEFAULT_LEVEL)
                {
                    if (string.IsNullOrEmpty(Map.MapFileName))
                    {
                        dy = (32 * 15 - dh) / 2;
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
                if (dx == SRC.DEFAULT_LEVEL)
                {
                    dx = (MainPWidth - dw) / 2;
                    if (MainWidth % 2 == 0)
                    {
                        dx = dx - 16;
                    }
                }

                if (dy == SRC.DEFAULT_LEVEL)
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
                if (dx == SRC.DEFAULT_LEVEL)
                {
                    dx = (MainPWidth - dw) / 2;
                }

                if (dy == SRC.DEFAULT_LEVEL)
                {
                    dy = (MainPHeight - dh) / 2;
                }
            }

            // 描画先が画面外の場合や描画サイズが0の場合は画像のロードのみを行う
            if (dx >= SrcFormatter.PixelsToTwipsX(pic.Width) | dy >= SrcFormatter.PixelsToTwipsY(pic.Height) | dx + dw <= 0 | dy + dh <= 0 | dw <= 0 | dh <= 0)
            {
                load_only = true;
            }

            // 描画を最適化するため、描画方法を細かく分けている。
            // 描画方法は以下の通り。
            // (1) BitBltでそのまま描画 (拡大処理なし、透過処理なし)
            // (2) 拡大画像を作ってからバッファリングして描画 (拡大処理あり、透過処理なし)
            // (3) 拡大画像を作らずにStretchBltで直接拡大描画 (拡大処理あり、透過処理なし)
            // (4) TransparentBltで拡大透過描画 (拡大処理あり、透過処理あり)
            // (5) 原画像をそのまま透過描画 (拡大処理なし、透過処理あり)
            // (6) 拡大画像を作ってからバッファリングして透過描画 (拡大処理あり、透過処理あり)
            // (7) 拡大画像を作ってからバッファリングせずに透過描画 (拡大処理あり、透過処理あり)
            // (8) 拡大画像を作らずにStretchBltで直接拡大透過描画 (拡大処理あり、透過処理あり)

            // 画面に描画する
            if (!transparent & dw == orig_width & dh == orig_height)
            {
                // 原画像をそのまま描画

                // 描画をキャンセル？
                if (load_only)
                {
                    DrawPictureRet = true;
                    return DrawPictureRet;
                }

                // 画像を描画先に描画
                ret = BitBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, SRCCOPY);
            }
            else if (SRC.KeepStretchedImage & !transparent & (!found_orig | load_only) & dw <= 480 & dh <= 480)
            {
                // 拡大画像を作成し、バッファリングして描画

                // 拡大画像に使用するpicBufを選択
                i = GUI.GetPicBuf(display_byte_pixel * dw * dh);
                PicBufFname[i] = fname;
                PicBufIsMask[i] = false;
                PicBufOption[i] = pic_option;
                PicBufOption2[i] = pic_option2;
                PicBufDW[i] = (short)dw;
                PicBufDH[i] = (short)dh;
                PicBufSX[i] = (short)sx;
                PicBufSY[i] = (short)sy;
                PicBufSW[i] = (short)sw;
                PicBufSH[i] = (short)sh;
                // Debug.Print "Use " & Format$(i) & " As Stretched"

                // バッファの初期化
                stretched_pic = MainForm.picBuf(i);
                {
                    var withBlock9 = stretched_pic;
                    withBlock9.Image = Image.FromFile("");
                    withBlock9.Width = (int)SrcFormatter.TwipsToPixelsX(dw);
                    withBlock9.Height = (int)SrcFormatter.TwipsToPixelsY(dh);
                }

                // バッファに拡大した画像を保存
                ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);

                // 描画をキャンセル？
                if (load_only)
                {
                    DrawPictureRet = true;
                    return DrawPictureRet;
                }

                // 拡大した画像を描画先に描画
                ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCCOPY);
            }
            else if (!transparent)
            {
                // 拡大画像を作らずにStretchBltで直接拡大描画

                // 描画をキャンセル？
                if (load_only)
                {
                    DrawPictureRet = true;
                    return DrawPictureRet;
                }

                // 拡大した画像を描画先に描画
                ret = StretchBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);
            }
            else if (SRC.UseTransparentBlt & (dw != orig_width | dh != orig_height) & found_orig & !load_only & (dw * dh < 40000 | orig_width * orig_height > 40000))
            {
                // TransparentBltの方が高速に描画できる場合に限り
                // TransparentBltを使って拡大透過描画

                // 描画をキャンセル？
                if (load_only)
                {
                    DrawPictureRet = true;
                    return DrawPictureRet;
                }

                // 画像を描画先に透過描画
                ret = TransparentBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, BGColor);
            }
            else if (dw == orig_width & dh == orig_height)
            {
                // 原画像をそのまま透過描画

                // 以前使用したマスク画像が利用可能？
                // UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                mask_pic.Image = null;
                var loopTo12 = (short)(SRC.ImageBufferSize - 1);
                for (i = 0; i <= loopTo12; i++)
                {
                    // 同じファイル？
                    if ((PicBufFname[i] ?? "") == (fname ?? ""))
                    {
                        // オプションも同じ？
                        if (PicBufIsMask[i] & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & PicBufDW[i] == orig_width & PicBufDH[i] == orig_height & PicBufSX[i] == sx & PicBufSX[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                        {
                            // 以前使用したマスク画像をそのまま利用
                            UsePicBuf(i);
                            mask_pic = MainForm.picBuf(i);
                            // Debug.Print "Reuse " & Format$(i) & " As Mask"
                            break;
                        }
                    }
                }

                if (mask_pic is null)
                {
                    // マスク画像を新規に作成

                    // マスク画像に使用するpicBufを選択
                    i = GUI.GetPicBuf(display_byte_pixel * dw * dh);
                    PicBufFname[i] = fname;
                    PicBufIsMask[i] = true;
                    PicBufOption[i] = "";
                    PicBufOption2[i] = pic_option2;
                    PicBufDW[i] = (short)orig_width;
                    PicBufDH[i] = (short)orig_height;
                    PicBufSX[i] = (short)sx;
                    PicBufSY[i] = (short)sy;
                    PicBufSW[i] = (short)sw;
                    PicBufSH[i] = (short)sh;
                    // Debug.Print "Use " & Format$(i) & " As Mask"

                    // バッファの初期化
                    mask_pic = MainForm.picBuf(i);
                    {
                        var withBlock10 = mask_pic;
                        withBlock10.Image = Image.FromFile("");
                        withBlock10.Width = (int)SrcFormatter.TwipsToPixelsX(orig_width);
                        withBlock10.Height = (int)SrcFormatter.TwipsToPixelsY(orig_height);
                    }

                    // マスク画像を作成
                    Graphics.MakeMask(ref orig_pic.hDC, ref mask_pic.hDC, ref orig_width, ref orig_height, ref BGColor);
                }

                // 描画をキャンセル？
                if (load_only)
                {
                    DrawPictureRet = true;
                    return DrawPictureRet;
                }

                // 画像を透過描画
                if (BGColor == ColorTranslator.ToOle(Color.White))
                {
                    // 背景色が白
                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCERASE);

                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, SRCINVERT);
                }
                else
                {
                    // 背景色が白以外
                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCAND);

                    ret = BitBlt(mask_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, SRCERASE);

                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCINVERT);

                    // マスク画像が再利用できないのでバッファを開放
                    ReleasePicBuf(i);
                }
            }
            else if (SRC.KeepStretchedImage & (!found_orig | load_only) & dw <= 480 & dh <= 480)
            {
                // 拡大画像を作成し、バッファリングして透過描画

                // 拡大画像用に使用するpicBufを選択
                i = GUI.GetPicBuf(display_byte_pixel * dw * dh);
                PicBufFname[i] = fname;
                PicBufIsMask[i] = false;
                PicBufOption[i] = pic_option;
                PicBufOption2[i] = pic_option2;
                PicBufDW[i] = (short)dw;
                PicBufDH[i] = (short)dh;
                PicBufSX[i] = (short)sx;
                PicBufSY[i] = (short)sy;
                PicBufSW[i] = (short)sw;
                PicBufSH[i] = (short)sh;
                // Debug.Print "Use " & Format$(i) & " As Stretched"

                // バッファの初期化
                stretched_pic = MainForm.picBuf(i);
                {
                    var withBlock11 = stretched_pic;
                    withBlock11.Image = Image.FromFile("");
                    withBlock11.Width = (int)SrcFormatter.TwipsToPixelsX(dw);
                    withBlock11.Height = (int)SrcFormatter.TwipsToPixelsY(dh);
                }

                // バッファに拡大した画像を保存
                ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);

                // 以前使用した拡大マスク画像が利用可能？
                // UPGRADE_NOTE: オブジェクト stretched_mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                stretched_mask_pic.Image = null;
                var loopTo13 = (short)(SRC.ImageBufferSize - 1);
                for (i = 0; i <= loopTo13; i++)
                {
                    // 同じファイル？
                    if ((PicBufFname[i] ?? "") == (fname ?? ""))
                    {
                        // オプションも同じ？
                        if (PicBufIsMask[i] & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & PicBufDW[i] == dw & PicBufDH[i] == dh & PicBufSX[i] == sx & PicBufSY[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                        {
                            // 以前使用した拡大マスク画像をそのまま利用
                            UsePicBuf(i);
                            stretched_mask_pic = MainForm.picBuf(i);
                            // Debug.Print "Reuse " & Format$(i) & " As StretchedMask"
                            break;
                        }
                    }
                }

                if (stretched_mask_pic is null)
                {
                    // 拡大マスク画像を新規に作成

                    // マスク画像用の領域を初期化
                    mask_pic = MainForm.picTmp;
                    {
                        var withBlock12 = mask_pic;
                        withBlock12.Image = Image.FromFile("");
                        withBlock12.Width = (int)SrcFormatter.TwipsToPixelsX(orig_width);
                        withBlock12.Height = (int)SrcFormatter.TwipsToPixelsY(orig_height);
                    }

                    // マスク画像を作成
                    Graphics.MakeMask(ref orig_pic.hDC, ref mask_pic.hDC, ref orig_width, ref orig_height, ref BGColor);

                    // 拡大マスク画像に使用するpicBufを選択
                    i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                    PicBufFname[i] = fname;
                    PicBufIsMask[i] = true;
                    PicBufOption[i] = "";
                    PicBufOption2[i] = pic_option2;
                    PicBufDW[i] = (short)dw;
                    PicBufDH[i] = (short)dh;
                    PicBufSX[i] = (short)sx;
                    PicBufSY[i] = (short)sy;
                    PicBufSW[i] = (short)sw;
                    PicBufSH[i] = (short)sh;
                    // Debug.Print "Use " & Format$(i) & " As StretchedMask"

                    // バッファを初期化
                    stretched_mask_pic = MainForm.picBuf(i);
                    {
                        var withBlock13 = stretched_mask_pic;
                        withBlock13.Image = Image.FromFile("");
                        withBlock13.Width = (int)SrcFormatter.TwipsToPixelsX(dw);
                        withBlock13.Height = (int)SrcFormatter.TwipsToPixelsY(dh);
                    }

                    // バッファに拡大したマスク画像を保存
                    ret = StretchBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);
                }

                // 描画をキャンセル？
                if (load_only)
                {
                    DrawPictureRet = true;
                    return DrawPictureRet;
                }

                // 画像を透過描画
                if (BGColor == ColorTranslator.ToOle(Color.White))
                {
                    // 背景色が白
                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCERASE);

                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCINVERT);
                }
                else
                {
                    // 背景色が白以外
                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCAND);

                    ret = BitBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, stretched_pic.hDC, 0, 0, SRCERASE);

                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCINVERT);

                    // 拡大マスク画像が再利用できないのでバッファを開放
                    ReleasePicBuf(i);
                }
            }
            else if (dw <= 480 & dh <= 480)
            {
                // 拡大画像を作成した後、バッファリングせずに透過描画

                // 拡大画像用の領域を作成
                stretched_pic = MainForm.picStretchedTmp(0);
                stretched_pic.Width = (int)SrcFormatter.TwipsToPixelsX(dw);
                stretched_pic.Height = (int)SrcFormatter.TwipsToPixelsY(dh);

                // バッファに拡大した画像を保存
                ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);

                // 以前使用したマスク画像が利用可能？
                // UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                mask_pic.Image = null;
                var loopTo14 = (short)(SRC.ImageBufferSize - 1);
                for (i = 0; i <= loopTo14; i++)
                {
                    // 同じファイル？
                    if ((PicBufFname[i] ?? "") == (fname ?? ""))
                    {
                        // オプションも同じ？
                        if (PicBufIsMask[i] & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & PicBufDW[i] == orig_width & PicBufDH[i] == orig_height & PicBufSX[i] == sx & PicBufSX[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                        {
                            // 以前使用したマスク画像をそのまま利用
                            UsePicBuf(i);
                            mask_pic = MainForm.picBuf(i);
                            // Debug.Print "Reuse " & Format$(i) & " As Mask"
                            break;
                        }
                    }
                }

                if (mask_pic is null)
                {
                    // 新規にマスク画像作成

                    // マスク画像に使用するpicBufを選択
                    i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                    PicBufFname[i] = fname;
                    PicBufIsMask[i] = true;
                    PicBufOption[i] = "";
                    PicBufOption2[i] = pic_option2;
                    PicBufDW[i] = (short)orig_width;
                    PicBufDH[i] = (short)orig_height;
                    PicBufSX[i] = (short)sx;
                    PicBufSY[i] = (short)sy;
                    PicBufSW[i] = (short)sw;
                    PicBufSH[i] = (short)sh;
                    // Debug.Print "Use " & Format$(i) & " As Mask"

                    // バッファを初期化
                    mask_pic = MainForm.picBuf(i);
                    {
                        var withBlock14 = mask_pic;
                        withBlock14.Width = (int)SrcFormatter.TwipsToPixelsX(orig_width);
                        withBlock14.Height = (int)SrcFormatter.TwipsToPixelsY(orig_height);
                    }

                    // マスク画像を作成
                    Graphics.MakeMask(ref orig_pic.hDC, ref mask_pic.hDC, ref orig_width, ref orig_height, ref BGColor);
                }

                // 拡大マスク画像用の領域を作成
                stretched_mask_pic = MainForm.picStretchedTmp(1);
                stretched_mask_pic.Image = Image.FromFile("");
                stretched_mask_pic.Width = (int)SrcFormatter.TwipsToPixelsX(dw);
                stretched_mask_pic.Height = (int)SrcFormatter.TwipsToPixelsY(dh);

                // マスク画像を拡大して拡大マスク画像を作成
                ret = StretchBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY);

                // 描画をキャンセル？
                if (load_only)
                {
                    DrawPictureRet = true;
                    return DrawPictureRet;
                }

                // 画像を透過描画
                if (BGColor == ColorTranslator.ToOle(Color.White))
                {
                    // 背景色が白
                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCERASE);

                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCINVERT);
                }
                else
                {
                    // 背景色が白以外
                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCAND);

                    ret = BitBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, stretched_pic.hDC, 0, 0, SRCERASE);

                    ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCINVERT);
                }

                // 使用した一時画像領域を開放
                {
                    var withBlock15 = MainForm.picStretchedTmp(0);
                    withBlock15.Picture = Image.FromFile("");
                    withBlock15.width = 32;
                    withBlock15.Height = 32;
                }
                {
                    var withBlock16 = MainForm.picStretchedTmp(1);
                    withBlock16.Picture = Image.FromFile("");
                    withBlock16.width = 32;
                    withBlock16.Height = 32;
                }
            }
            else
            {
                // 拡大画像を作成せず、StretchBltで直接拡大透過描画

                // 以前使用したマスク画像が利用可能？
                // UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                mask_pic.Image = null;
                var loopTo11 = (short)(SRC.ImageBufferSize - 1);
                for (i = 0; i <= loopTo11; i++)
                {
                    // 同じファイル？
                    if ((PicBufFname[i] ?? "") == (fname ?? ""))
                    {
                        // オプションも同じ？
                        if (PicBufIsMask[i] & (PicBufOption2[i] ?? "") == (pic_option2 ?? "") & PicBufDW[i] == orig_width & PicBufDH[i] == orig_height & PicBufSX[i] == sx & PicBufSX[i] == sy & PicBufSW[i] == sw & PicBufSH[i] == sh)
                        {
                            // 以前使用したマスク画像をそのまま利用
                            UsePicBuf(i);
                            mask_pic = MainForm.picBuf(i);
                            // Debug.Print "Reuse " & Format$(i) & " As Mask"
                            break;
                        }
                    }
                }

                if (mask_pic is null)
                {
                    // 新規にマスク画像作成

                    // マスク画像に使用するpicBufを選択
                    i = GUI.GetPicBuf(display_byte_pixel * orig_width * orig_height);
                    PicBufFname[i] = fname;
                    PicBufIsMask[i] = true;
                    PicBufOption[i] = "";
                    PicBufOption2[i] = pic_option2;
                    PicBufDW[i] = (short)orig_width;
                    PicBufDH[i] = (short)orig_height;
                    PicBufSX[i] = (short)sx;
                    PicBufSY[i] = (short)sy;
                    PicBufSW[i] = (short)sw;
                    PicBufSH[i] = (short)sh;
                    // Debug.Print "Use " & Format$(i) & " As Mask"

                    // バッファを初期化
                    mask_pic = MainForm.picBuf(i);
                    mask_pic.Width = (int)SrcFormatter.TwipsToPixelsX(orig_width);
                    mask_pic.Height = (int)SrcFormatter.TwipsToPixelsY(orig_height);

                    // マスク画像を作成
                    Graphics.MakeMask(ref orig_pic.hDC, ref mask_pic.hDC, ref orig_width, ref orig_height, ref BGColor);
                }

                // 描画をキャンセル？
                if (load_only)
                {
                    DrawPictureRet = true;
                    return DrawPictureRet;
                }

                // 画像を透過描画
                if (BGColor == ColorTranslator.ToOle(Color.White))
                {
                    // 背景色が白
                    ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCERASE);

                    ret = StretchBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCINVERT);
                }
                else
                {
                    // 背景色が白以外
                    ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCAND);

                    ret = BitBlt(mask_pic.hDC, 0, 0, orig_width, orig_width, orig_pic.hDC, 0, 0, SRCERASE);

                    ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCINVERT);

                    // マスク画像が再利用できないのでバッファを開放
                    ReleasePicBuf(i);
                }
            }

            DrewPicture:
            ;
            if (permanent)
            {
                // 背景への描き込み
                Map.IsMapDirty = true;
                {
                    var withBlock17 = MainForm;
                    // マスク入り背景画像画面にも画像を描き込む
                    ret = BitBlt(withBlock17.picMaskedBack.hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY);
                    var loopTo15 = (short)((dx + dw - 1) / 32);
                    for (i = (short)(dx / 32); i <= loopTo15; i++)
                    {
                        var loopTo16 = (short)((dy + dh - 1) / 32);
                        for (j = (short)(dy / 32); j <= loopTo16; j++)
                        {
                            ret = BitBlt(withBlock17.picMaskedBack.hDC, 32 * (int)i, 32 * (int)j, 32, 32, withBlock17.picMask.hDC, 0, 0, SRCAND);
                            ret = BitBlt(withBlock17.picMaskedBack.hDC, 32 * (int)i, 32 * (int)j, 32, 32, withBlock17.picMask2.hDC, 0, 0, SRCINVERT);
                        }
                    }
                }
            }
            else if (!on_msg_window & !on_status_window)
            {
                // 表示画像を消去する際に使う描画領域を設定
                PaintedAreaX1 = (short)GeneralLib.MinLng(PaintedAreaX1, GeneralLib.MaxLng(dx, 0));
                PaintedAreaY1 = (short)GeneralLib.MinLng(PaintedAreaY1, GeneralLib.MaxLng(dy, 0));
                PaintedAreaX2 = (short)GeneralLib.MaxLng(PaintedAreaX2, GeneralLib.MinLng(dx + dw, MainPWidth - 1));
                PaintedAreaY2 = (short)GeneralLib.MaxLng(PaintedAreaY2, GeneralLib.MinLng(dy + dh, MainPHeight - 1));
                IsPictureDrawn = true;
                IsPictureVisible = true;
                IsCursorVisible = false;
                if (keep_picture)
                {
                    // picMain(1)にも描画
                    ret = BitBlt(MainForm.picMain(1).hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY);
                }
            }

            DrawPictureRet = true;
            return DrawPictureRet;
        }

        // 画像バッファを作成
        public static void MakePicBuf()
        {
            short i;

            // 画像バッファ用のPictureBoxを動的に生成する
            {
                var withBlock = MainForm;
                var loopTo = (short)(SRC.ImageBufferSize - 1);
                for (i = 1; i <= loopTo; i++)
                    Load(withBlock.picBuf(i));
            }

            // 画像バッファ管理用配列を初期化
            PicBufDate = new int[(SRC.ImageBufferSize + 1)];
            PicBufSize = new int[(SRC.ImageBufferSize + 1)];
            PicBufFname = new string[SRC.ImageBufferSize];
            PicBufOption = new string[SRC.ImageBufferSize];
            PicBufOption2 = new string[SRC.ImageBufferSize];
            PicBufDW = new short[SRC.ImageBufferSize];
            PicBufDH = new short[SRC.ImageBufferSize];
            PicBufSX = new short[SRC.ImageBufferSize];
            PicBufSY = new short[SRC.ImageBufferSize];
            PicBufSW = new short[SRC.ImageBufferSize];
            PicBufSH = new short[SRC.ImageBufferSize];
            PicBufIsMask = new bool[SRC.ImageBufferSize];
        }

        // 使用可能な画像バッファを検索
        private static short GetPicBuf(int buf_size = 0)
        {
            short GetPicBufRet = default;
            int total_size;
            short oldest_buf = default, used_buf_num = default;
            short i;
            int tmp;

            // 画像バッファの総サイズ及び使用されているバッファ数を調べる
            total_size = buf_size;
            var loopTo = (short)(SRC.ImageBufferSize - 1);
            for (i = 0; i <= loopTo; i++)
            {
                total_size = total_size + PicBufSize[i];
                if (!string.IsNullOrEmpty(PicBufFname[i]))
                {
                    used_buf_num = (short)(used_buf_num + 1);
                }
            }

            // 総サイズがMaxImageBufferByteSizeを超えてしまう場合は総サイズが
            // MaxImageBufferByteSize以下になるまでバッファを開放する。
            // ただし一度の描画で最大で5枚のバッファが使われるため、最新の4つの
            // バッファはキープしておく。
            while (total_size > SRC.MaxImageBufferByteSize & used_buf_num > 4)
            {
                // 最も長い間使われていないバッファを探す
                tmp = 100000000;
                var loopTo1 = (short)(SRC.ImageBufferSize - 1);
                for (i = 0; i <= loopTo1; i++)
                {
                    if (!string.IsNullOrEmpty(PicBufFname[i]))
                    {
                        if (PicBufDate[i] < tmp)
                        {
                            oldest_buf = i;
                            tmp = PicBufDate[i];
                        }
                    }
                }

                // バッファを開放
                ReleasePicBuf(oldest_buf);
                used_buf_num = (short)(used_buf_num - 1);

                // 総サイズ数を減少させる
                total_size = total_size - PicBufSize[oldest_buf];
                PicBufSize[oldest_buf] = 0;
            }

            // 最も長い間使われていないバッファを探す
            GetPicBufRet = 0;
            var loopTo2 = (short)(SRC.ImageBufferSize - 1);
            for (i = 1; i <= loopTo2; i++)
            {
                if (PicBufDate[i] < PicBufDate[GetPicBufRet])
                {
                    GetPicBufRet = i;
                }
            }

            // 画像のサイズを記録しておく
            PicBufSize[GetPicBufRet] = buf_size;

            // 使用することを記録する
            UsePicBuf(GetPicBufRet);
            return GetPicBufRet;
        }

        // 画像バッファを開放する
        private static void ReleasePicBuf(short idx)
        {
            PicBufFname[idx] = "";
            {
                var withBlock = MainForm.picBuf(idx);
                withBlock.Picture = Image.FromFile("");
                withBlock.width = 32;
                withBlock.Height = 32;
            }
        }

        // 画像バッファの使用記録をつける
        private static void UsePicBuf(short idx)
        {
            PicBufDateCount = PicBufDateCount + 1;
            PicBufDate[idx] = PicBufDateCount;
        }


        // === 文字列描画に関する処理 ===

        // メインウィンドウに文字列を表示する
        public static void DrawString(ref string msg, int X, int Y, bool without_cr = false)
        {
            short tx, ty;
            short prev_cx;
            PictureBox pic;
            Font sf;
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
                    Static font_smoothing As Integer

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
                    Static init_draw_string As Boolean

             */
            if (PermanentStringMode)
            {
                // 背景書き込み
                pic = MainForm.picBack;
                // フォント設定を変更
                {
                    var withBlock = MainForm.picBack;
                    withBlock.ForeColor = MainForm.picMain(0).ForeColor;
                    if (withBlock.Font.Name != MainForm.picMain(0).Font.Name)
                    {
                        sf = (Font)Control.DefaultFont.Clone();
                        sf = SrcFormatter.FontChangeName(sf, MainForm.picMain(0).Font.Name);
                        withBlock.Font = sf;
                    }
                    withBlock.Font.Size = MainForm.picMain(0).Font.Size;
                    withBlock.Font.Bold = MainForm.picMain(0).Font.Bold;
                    withBlock.Font.Italic = MainForm.picMain(0).Font.Italic;
                }
                {
                    var withBlock1 = MainForm.picMaskedBack;
                    withBlock1.ForeColor = MainForm.picMain(0).ForeColor;
                    if (withBlock1.Font.Name != MainForm.picMain(0).Font.Name)
                    {
                        sf = (Font)Control.DefaultFont.Clone();
                        sf = SrcFormatter.FontChangeName(sf, MainForm.picMain(0).Font.Name);
                        withBlock1.Font = sf;
                    }
                    withBlock1.Font.Size = MainForm.picMain(0).Font.Size;
                    withBlock1.Font.Bold = MainForm.picMain(0).Font.Bold;
                    withBlock1.Font.Italic = MainForm.picMain(0).Font.Italic;
                }
            }
            else
            {
                // 通常の書き込み
                pic = MainForm.picMain(0);
                SaveScreen();
            }

            // フォントがスムージング表示されているか参照
            if (!init_draw_string)
            {
                GUI.GetSystemParametersInfo((int)SPI_GETFONTSMOOTHING, 0, ref font_smoothing, 0);
                init_draw_string = true;
            }

            // フォントをスムージングするように設定
            if (font_smoothing == 0)
            {
                SetSystemParametersInfo(SPI_SETFONTSMOOTHING, 1, 0, 0);
            }
            // 現在のX位置を記録しておく
            prev_cx = pic.CurrentX;

            // 書き込み先の座標を求める
            if (HCentering)
            {
                pic.CurrentX = (SrcFormatter.PixelsToTwipsX(pic.Width) - pic.TextWidth(msg)) / 2;
            }
            else if (X != SRC.DEFAULT_LEVEL)
            {
                pic.CurrentX = X;
            }

            if (VCentering)
            {
                pic.CurrentY = (SrcFormatter.PixelsToTwipsY(pic.Height) - pic.TextHeight(msg)) / 2;
            }
            else if (Y != SRC.DEFAULT_LEVEL)
            {
                pic.CurrentY = Y;
            }
            tx = pic.CurrentX;
            ty = pic.CurrentY;
            if (!without_cr)
            {
                // 改行あり
                pic.Print(msg);

                // 背景書き込みの場合
                if (PermanentStringMode)
                {
                    {
                        var withBlock2 = MainForm.picMaskedBack;
                        withBlock2.CurrentX = tx;
                        withBlock2.CurrentY = ty;
                    }
                    MainForm.picMaskedBack.Print(msg);
                    Map.IsMapDirty = true;
                }

                // 保持オプション使用時
                if (KeepStringMode)
                {
                    {
                        var withBlock3 = MainForm.picMain(1);
                        withBlock3.CurrentX = tx;
                        withBlock3.CurrentY = ty;
                        withBlock3.ForeColor = ColorTranslator.ToOle(pic.ForeColor);
                        if (withBlock3.Font.Name != pic.Font.Name)
                        {
                            sf = (Font)Control.DefaultFont.Clone();
                            sf = SrcFormatter.FontChangeName(sf, pic.Font.Name);
                            withBlock3.Font = sf;
                        }
                        withBlock3.Font.Size = pic.Font.SizeInPoints;
                        withBlock3.Font.Bold = pic.Font.Bold;
                        withBlock3.Font.Italic = pic.Font.Italic;
                    }
                    MainForm.picMain(1).Print(msg);
                }

                // 次回の書き込みのため、X座標位置を設定し直す
                if (X != SRC.DEFAULT_LEVEL)
                {
                    pic.CurrentX = X;
                }
                else
                {
                    pic.CurrentX = prev_cx;
                }
            }
            else
            {
                // 改行なし
                pic.Print(msg);

                // 背景書き込みの場合
                if (PermanentStringMode)
                {
                    {
                        var withBlock4 = MainForm.picMaskedBack;
                        withBlock4.CurrentX = tx;
                        withBlock4.CurrentY = ty;
                    }
                    MainForm.picMaskedBack.Print(msg);
                    Map.IsMapDirty = true;
                }

                // 保持オプション使用時
                if (KeepStringMode)
                {
                    {
                        var withBlock5 = MainForm.picMain(1);
                        withBlock5.CurrentX = tx;
                        withBlock5.CurrentY = ty;
                    }
                    MainForm.picMain(1).Print(msg);
                }
            }

            // フォントのスムージングに関する設定を元に戻す
            if (font_smoothing == 0)
            {
                SetSystemParametersInfo(SPI_SETFONTSMOOTHING, 0, 0, 0);
            }

            if (!PermanentStringMode)
            {
                IsPictureVisible = true;
                PaintedAreaX1 = 0;
                PaintedAreaY1 = 0;
                PaintedAreaX2 = (short)(MainPWidth - 1);
                PaintedAreaY2 = (short)(MainPHeight - 1);
            }
        }

        // メインウィンドウに文字列を表示 (システムメッセージ)
        public static void DrawSysString(short X, short Y, ref string msg, bool without_refresh = false)
        {
            int prev_color;
            string prev_name;
            short prev_size;
            bool is_bold;
            bool is_italic;
            Font sf;

            // 表示位置が画面外？
            if (X < MapX - MainWidth / 2 | MapX + MainWidth / 2 < X | Y < MapY - MainHeight / 2 | MapY + MainHeight / 2 < Y)
            {
                return;
            }

            SaveScreen();

            {
                var withBlock = MainForm.picMain(0);
                // 現在のフォント設定を保存
                prev_color = withBlock.ForeColor;
                prev_size = withBlock.Font.Size;
                prev_name = withBlock.Font.Name;
                is_bold = withBlock.Font.Bold;
                is_italic = withBlock.Font.Italic;

                // フォント設定をシステム用に切り替え
                withBlock.ForeColor = ColorTranslator.ToOle(Color.Black);
                withBlock.FontTransparent = false;
                if (withBlock.Font.Name != "ＭＳ Ｐ明朝")
                {
                    sf = (Font)Control.DefaultFont.Clone();
                    sf = SrcFormatter.FontChangeName(sf, "ＭＳ Ｐ明朝");
                    withBlock.Font = sf;
                }
                {
                    var withBlock1 = withBlock.Font;
                    if (SRC.BattleAnimation)
                    {
                        withBlock1.Size = 9;
                        withBlock1.Bold = true;
                    }
                    else
                    {
                        withBlock1.Size = 8;
                        withBlock1.Bold = false;
                    }
                    withBlock1.Italic = false;
                }

                // メッセージの書き込み
                withBlock.CurrentX = MapToPixelX(X) + (32 - withBlock.TextWidth(msg)) / 2 - 1;
                withBlock.CurrentY = MapToPixelY((short)(Y + 1)) - withBlock.TextHeight(msg);
                MainForm.picMain(0).Print(msg);

                // フォント設定を元に戻す
                withBlock.ForeColor = prev_color;
                withBlock.FontTransparent = true;
                if (withBlock.Font.Name != prev_name)
                {
                    sf = (Font)Control.DefaultFont.Clone();
                    sf = SrcFormatter.FontChangeName(sf, prev_name);
                    withBlock.Font = sf;
                }
                {
                    var withBlock2 = withBlock.Font;
                    withBlock2.Size = prev_size;
                    withBlock2.Bold = is_bold;
                    withBlock2.Italic = is_italic;
                }

                // 表示を更新
                if (!without_refresh)
                {
                    withBlock.Refresh();
                }

                PaintedAreaX1 = (short)GeneralLib.MinLng(PaintedAreaX1, MapToPixelX(X) - 4);
                PaintedAreaY1 = (short)GeneralLib.MaxLng(PaintedAreaY1, MapToPixelY(Y) + 16);
                PaintedAreaX2 = (short)GeneralLib.MinLng(PaintedAreaX2, MapToPixelX(X) + 36);
                PaintedAreaY2 = (short)GeneralLib.MaxLng(PaintedAreaY2, MapToPixelY(Y) + 32);
            }
        }


        // === 画像消去に関する処理 ===

        // 描画した画像を消去できるように元画像を保存する
        public static void SaveScreen()
        {
            int ret;
            if (!ScreenIsSaved)
            {
                // 画像をpicMain(1)に保存
                {
                    var withBlock = MainForm;
                    ret = BitBlt(withBlock.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, withBlock.picMain(0).hDC, 0, 0, SRCCOPY);
                }

                ScreenIsSaved = true;
            }
        }

        // 描画したグラフィックを消去
        public static void ClearPicture()
        {
            short pawidth, paheight;
            int ret;
            if (!ScreenIsSaved)
            {
                return;
            }

            IsPictureVisible = false;
            IsCursorVisible = false;
            pawidth = (short)(PaintedAreaX2 - PaintedAreaX1 + 1);
            paheight = (short)(PaintedAreaY2 - PaintedAreaY1 + 1);
            if (pawidth < 1 | paheight < 1)
            {
                return;
            }

            {
                var withBlock = MainForm;
                ret = BitBlt(withBlock.picMain(0).hDC, PaintedAreaX1, PaintedAreaY1, pawidth, paheight, withBlock.picMain(1).hDC, PaintedAreaX1, PaintedAreaY1, SRCCOPY);
            }
        }

        // 描画したグラフィックの一部を消去
        public static void ClearPicture2(int x1, int y1, int x2, int y2)
        {
            int ret;
            if (!ScreenIsSaved)
            {
                return;
            }

            {
                var withBlock = MainForm;
                ret = BitBlt(withBlock.picMain(0).hDC, x1, y1, x2 - x1 + 1, y2 - y1 + 1, withBlock.picMain(1).hDC, x1, y1, SRCCOPY);
            }
        }


        // === 画面ロックに関する処理 ===

        // ＧＵＩをロックし、プレイヤーからの入力を無効にする
        public static void LockGUI()
        {
            IsGUILocked = true;
            {
                var withBlock = MainForm;
                withBlock.VScroll.Enabled = false;
                withBlock.HScroll.Enabled = false;
            }
        }

        // ＧＵＩのロックを解除し、プレイヤーからの入力を有効にする
        public static void UnlockGUI()
        {
            IsGUILocked = false;
            {
                var withBlock = MainForm;
                withBlock.VScroll.Enabled = true;
                withBlock.HScroll.Enabled = true;
            }
        }


        // === マウスカーソルの自動移動に関する処理 ===

        // 現在のマウスカーソルの位置を記録
        public static void SaveCursorPos()
        {
            var PT = default(POINTAPI);
            GetCursorPos(ref PT);
            PrevCursorX = (short)PT.X;
            PrevCursorY = (short)PT.Y;
            NewCursorX = 0;
            NewCursorY = 0;
        }

        // マウスカーソルを移動する
        public static void MoveCursorPos(ref string cursor_mode, Unit t = null)
        {
            int i, tx, ty, num;
            int ret;
            bool prev_lock;
            var PT = default(POINTAPI);

            // マウスカーソルの位置を収得
            GetCursorPos(ref PT);

            // 現在の位置を記録しておく
            if (PrevCursorX == 0 & cursor_mode != "メッセージウィンドウ")
            {
                SaveCursorPos();
            }

            // カーソル自動移動
            if (t is null)
            {
                if (cursor_mode == "メッセージウィンドウ")
                {
                    // メッセージウィンドウまで移動
                    {
                        var withBlock = My.MyProject.Forms.frmMessage;
                        if (PT.X < (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + 0.05d * SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX())
                        {
                            tx = (int)((long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + 0.05d * SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX());
                        }
                        else if (PT.X > (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + 0.95d * SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX())
                        {
                            tx = (int)((long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + 0.95d * SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX());
                        }
                        else
                        {
                            tx = PT.X;
                        }

                        if (PT.Y < (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY() - withBlock.ClientRectangle.Height + withBlock.picMessage.Top)
                        {
                            ty = (int)((long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY() - withBlock.ClientRectangle.Height + withBlock.picMessage.Top);
                        }
                        else if (PT.Y > (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + 0.9d * SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY())
                        {
                            ty = (int)((long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + 0.9d * SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY());
                        }
                        else
                        {
                            ty = PT.Y;
                        }
                    }
                }
                else
                {
                    // リストボックスまで移動
                    {
                        var withBlock1 = My.MyProject.Forms.frmListBox;
                        if (PT.X < (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + 0.1d * SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX())
                        {
                            tx = (int)((long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + 0.1d * SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX());
                        }
                        else if (PT.X > (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + 0.9d * SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX())
                        {
                            tx = (int)((long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + 0.9d * SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX());
                        }
                        else
                        {
                            tx = PT.X;
                        }

                        // 選択するアイテム
                        if (cursor_mode == "武器選択")
                        {
                            // 武器選択の場合は選択可能な最後のアイテムに
                            i = withBlock1.lstItems.Items.Count;
                            do
                            {
                                if (!ListItemFlag[i] & Strings.InStr(SrcFormatter.GetItemString(withBlock1.lstItems, i), "援護攻撃：") == 0)
                                {
                                    break;
                                }

                                i = i - 1;
                            }
                            while (i > 1);
                        }
                        else
                        {
                            // そうでなければ最初のアイテムに
                            i = withBlock1.lstItems.TopIndex + 1;
                        }

                        ty = (int)((long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() + (long)SrcFormatter.PixelsToTwipsY(withBlock1.Height) / (long)SrcFormatter.TwipsPerPixelY() - withBlock1.ClientRectangle.Height + withBlock1.lstItems.Top + 16 * (i - withBlock1.lstItems.TopIndex) - 8L);
                    }
                }
            }
            else
            {
                // ユニット上まで移動
                {
                    var withBlock2 = MainForm;
                    // MOD START 240a
                    // If MainWidth = 15 Then
                    // tx = .Left \ Screen.TwipsPerPixelX _
                    // '                    + 32 * (t.X - (MapX - MainWidth \ 2)) + 24
                    // ty = .Top \ Screen.TwipsPerPixelY _
                    // '                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
                    // '                    + 32 * (t.Y - (MapY - MainHeight \ 2)) + 20
                    // Else
                    // tx = .Left \ Screen.TwipsPerPixelX _
                    // '                    + 32 * (t.X - (MapX - MainWidth \ 2)) - 14
                    // ty = .Top \ Screen.TwipsPerPixelY _
                    // '                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
                    // '                    + 32 * (t.Y - (MapY - MainHeight \ 2)) + 16
                    // End If
                    if (NewGUIMode)
                    {
                        tx = (int)((long)SrcFormatter.PixelsToTwipsX(withBlock2.Left) / (long)SrcFormatter.TwipsPerPixelX() + 32 * (t.x - (MapX - MainWidth / 2)) + 4L);
                        ty = (int)((long)SrcFormatter.PixelsToTwipsY(withBlock2.Top) / (long)SrcFormatter.TwipsPerPixelY() + (long)SrcFormatter.PixelsToTwipsY(withBlock2.Height) / (long)SrcFormatter.TwipsPerPixelY() - SrcFormatter.PixelsToTwipsY(withBlock2.ClientRectangle.Height) + 32 * (t.y - (MapY - MainHeight / 2)) + 16d);
                    }
                    else
                    {
                        tx = (int)((long)SrcFormatter.PixelsToTwipsX(withBlock2.Left) / (long)SrcFormatter.TwipsPerPixelX() + 32 * (t.x - (MapX - MainWidth / 2)) + 24L);
                        ty = (int)((long)SrcFormatter.PixelsToTwipsY(withBlock2.Top) / (long)SrcFormatter.TwipsPerPixelY() + (long)SrcFormatter.PixelsToTwipsY(withBlock2.Height) / (long)SrcFormatter.TwipsPerPixelY() - SrcFormatter.PixelsToTwipsY(withBlock2.ClientRectangle.Height) + 32 * (t.y - (MapY - MainHeight / 2)) + 20d);
                    }
                    // MOD  END  240a
                }
            }

            // 何回に分けて移動するか計算
            num = (int)((long)Math.Sqrt(Math.Pow(tx - PT.X, 2d) + Math.Pow(ty - PT.Y, 2d)) / 25L + 1L);

            // カーソルを移動
            prev_lock = IsGUILocked;
            IsGUILocked = true;
            Status.IsStatusWindowDisabled = true;
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
            {
                ret = SetCursorPos((tx * i + PT.X * (num - i)) / num, (ty * i + PT.Y * (num - i)) / num);
                Sleep(10);
            }

            Application.DoEvents();
            Status.IsStatusWindowDisabled = false;
            IsGUILocked = prev_lock;

            // 新しいカーソル位置を記録
            if (NewCursorX == 0)
            {
                NewCursorX = (short)tx;
                NewCursorY = (short)ty;
            }
        }

        // マウスカーソルを元の位置に戻す
        public static void RestoreCursorPos()
        {
            short i, tx, ty, num;
            int ret;
            var PT = default(POINTAPI);

            // ユニットが選択されていればその場所まで戻す
            if (Commands.SelectedUnit is object)
            {
                if (Commands.SelectedUnit.Status == "出撃")
                {
                    string argcursor_mode = "ユニット選択";
                    MoveCursorPos(ref argcursor_mode, Commands.SelectedUnit);
                    return;
                }
            }

            // 戻るべき位置が設定されていない？
            if (PrevCursorX == 0 & PrevCursorY == 0)
            {
                return;
            }

            // 現在のカーソル位置収得
            GetCursorPos(ref PT);

            // 以前の位置までカーソル自動移動
            {
                var withBlock = My.MyProject.Forms.frmListBox;
                tx = PrevCursorX;
                ty = PrevCursorY;
                num = (short)((long)Math.Sqrt(Math.Pow(tx - PT.X, 2d) + Math.Pow(ty - PT.Y, 2d)) / 50L + 1L);
                var loopTo = num;
                for (i = 1; i <= loopTo; i++)
                {
                    ret = SetCursorPos((tx * i + PT.X * (num - i)) / num, (ty * i + PT.Y * (num - i)) / num);
                    Application.DoEvents();
                    Sleep(10);
                }
            }

            // 戻り位置を初期化
            PrevCursorX = 0;
            PrevCursorY = 0;
        }


        // === タイトル画面表示に関する処理 ===

        // タイトル画面を表示
        public static void OpenTitleForm()
        {
            Load(My.MyProject.Forms.frmTitle);
            My.MyProject.Forms.frmTitle.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(My.MyProject.Forms.frmTitle.Width)) / 2d);
            My.MyProject.Forms.frmTitle.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(My.MyProject.Forms.frmTitle.Height)) / 2d);
            My.MyProject.Forms.frmTitle.Show();
            My.MyProject.Forms.frmTitle.Refresh();
        }

        // タイトル画面を閉じる
        public static void CloseTitleForm()
        {
            My.MyProject.Forms.frmTitle.Close();
            // UPGRADE_NOTE: オブジェクト frmTitle をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            My.MyProject.Forms.frmTitle = null;
        }


        // === 「Now Loading...」表示に関する処理 ===

        // 「Now Loading...」の画面を表示
        public static void OpenNowLoadingForm()
        {
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;
            Load(My.MyProject.Forms.frmNowLoading);
            {
                var withBlock = My.MyProject.Forms.frmNowLoading;
                withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
                withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);
                withBlock.Show();
                withBlock.Label1.Refresh();
            }
        }

        // 「Now Loading...」の画面を消去
        public static void CloseNowLoadingForm()
        {
            My.MyProject.Forms.frmNowLoading.Close();
            // UPGRADE_NOTE: オブジェクト frmNowLoading をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            My.MyProject.Forms.frmNowLoading = null;
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
        }

        // 「Now Loading...」のバーを１段階進行させる
        public static void DisplayLoadingProgress()
        {
            My.MyProject.Forms.frmNowLoading.Progress();
            Application.DoEvents();
        }

        // 「Now Loading...」のバーの長さを設定
        public static void SetLoadImageSize(short new_size)
        {
            {
                var withBlock = My.MyProject.Forms.frmNowLoading;
                withBlock.Value = 0;
                withBlock.Size_Renamed = new_size;
            }
        }


        // === 画面の解像度変更 ===

        public static void ChangeDisplaySize(short w, short h)
        {
            var dm = default(DEVMODE);
            int ret;
            ;

            // DEVMODE構造体を初期化
            dm.dmSize = (short)Strings.Len(dm);

            // 現在のディスプレイ設定を参照
            ret = GUI.EnumDisplaySettings(ref Constants.vbNullString, (int)ENUM_CURRENT_SETTINGS, ref dm);
            if (w != 0 & h != 0)
            {
                // 画面の解像度を w x h に変更する場合

                // 現在の解像度を記録しておく
                orig_width = (short)dm.dmPelsWidth;
                orig_height = (short)dm.dmPelsHeight;
                if (dm.dmPelsWidth == w & dm.dmPelsHeight == h)
                {
                    // 既に使用したい解像度になっていればそのまま終了
                    return;
                }

                // 画面の解像度を w x h に変更
                dm.dmPelsWidth = w;
                dm.dmPelsHeight = h;
            }
            else
            {
                // 画面の解像度を元の解像度に戻す場合

                if (Conversions.ToBoolean(Conversions.ToShort((int)orig_width == 0) & orig_height))
                {
                    // 解像度を変更していなければ終了
                    return;
                }

                if (dm.dmPelsWidth == (int)orig_width & dm.dmPelsHeight == (int)orig_width)
                {
                    // 解像度が変化していなければそのまま終了
                    return;
                }

                // 画面の解像度を元に戻す
                ret = ChangeDisplaySettings(ref VariantType.Null, 0);
                return;
            }

            // 解像度を変更可能かどうか調べる
            // UPGRADE_WARNING: オブジェクト dm の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            ret = ChangeDisplaySettings(ref dm, CDS_TEST);
            if (ret != DISP_CHANGE_SUCCESSFUL)
            {
                return;
            }

            // 解像度を実際に変更する
            // MOD START MARGE
            // If GetWinVersion() >= 5 Then
            if (GeneralLib.GetWinVersion() >= 501)
            {
                // MOD END MARGE
                // UPGRADE_WARNING: オブジェクト dm の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                ret = ChangeDisplaySettings(ref dm, CDS_FULLSCREEN);
            }
            else
            {
                // UPGRADE_WARNING: オブジェクト dm の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                ret = ChangeDisplaySettings(ref dm, 0);
            }

            switch (ret)
            {
                case DISP_CHANGE_SUCCESSFUL:
                    {
                        // 成功！
                        return;
                    }

                case DISP_CHANGE_RESTART:
                    {
                        // 再起動が必要な場合はあきらめてもとの解像度に戻す
                        ret = ChangeDisplaySettings(ref VariantType.Null, 0);
                        break;
                    }
            }
        }


        // === その他 ===

        // エラーメッセージを表示
        public static void ErrorMessage(ref string msg)
        {
            int ret;

            Load(My.MyProject.Forms.frmErrorMessage);
            {
                var withBlock = My.MyProject.Forms.frmErrorMessage;
                ret = SetWindowPos(withBlock.Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
                withBlock.txtMessage.Text = msg;
                withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
                withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);
                withBlock.Show();
            }

            // メインウィンドウのクローズが行えるようにモーダルモードは使用しない
            while (My.MyProject.Forms.frmErrorMessage.Visible)
            {
                Application.DoEvents();
                Sleep(200);
            }

            My.MyProject.Forms.frmErrorMessage.Close();
            // UPGRADE_NOTE: オブジェクト frmErrorMessage をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            My.MyProject.Forms.frmErrorMessage = null;
        }

        // データ読み込み時のエラーメッセージを表示する
        public static void DataErrorMessage(ref string msg, ref string fname, short line_num, ref string line_buf, ref string dname)
        {
            string err_msg;

            // エラーが発生したファイル名と行番号
            err_msg = fname + "：" + line_num + "行目" + Constants.vbCr + Constants.vbLf;

            // エラーが発生したデータ名
            if (Strings.Len(dname) > 0)
            {
                err_msg = err_msg + dname + "のデータが不正です。" + Constants.vbCr + Constants.vbLf;
            }

            // エラーの原因
            if (Strings.Len(msg) > 0)
            {
                err_msg = err_msg + msg + Constants.vbCr + Constants.vbLf;
            }

            // なにも指定されていない？
            if (string.IsNullOrEmpty(dname) & string.IsNullOrEmpty(msg))
            {
                err_msg = err_msg + "データが不正です。" + Constants.vbCr + Constants.vbLf;
            }

            // エラーが発生したデータ行
            err_msg = err_msg + line_buf;

            // エラーメッセージを表示
            ErrorMessage(ref err_msg);
        }


        // マウスの右ボタンが押されているか(キャンセル)判定
        public static bool IsRButtonPressed(bool ignore_message_wait = false)
        {
            bool IsRButtonPressedRet = default;
            var PT = default(POINTAPI);

            // メッセージがウエイト無しならスキップ
            if (!ignore_message_wait & MessageWait == 0)
            {
                IsRButtonPressedRet = true;
                return IsRButtonPressedRet;
            }

            // メインウインドウ上でマウスボタンを押した場合
            if (MainForm.Handle.ToInt32() == GetForegroundWindow())
            {
                GetCursorPos(ref PT);
                {
                    var withBlock = MainForm;
                    if ((long)SrcFormatter.PixelsToTwipsX(withBlock.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X & PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX() & (long)SrcFormatter.PixelsToTwipsY(withBlock.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y & PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY())
                    {
                        if ((GetAsyncKeyState(RButtonID) & 0x8000) != 0)
                        {
                            // 右ボタンでスキップ
                            IsRButtonPressedRet = true;
                            return IsRButtonPressedRet;
                        }
                    }
                }
            }
            // メッセージウインドウ上でマウスボタンを押した場合
            else if (My.MyProject.Forms.frmMessage.Handle.ToInt32() == GetForegroundWindow())
            {
                GetCursorPos(ref PT);
                {
                    var withBlock1 = My.MyProject.Forms.frmMessage;
                    if ((long)SrcFormatter.PixelsToTwipsX(withBlock1.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X & PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX() & (long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y & PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock1.Top) + SrcFormatter.PixelsToTwipsY(withBlock1.Height)) / (long)SrcFormatter.TwipsPerPixelY())
                    {
                        if ((GetAsyncKeyState(RButtonID) & 0x8000) != 0)
                        {
                            // 右ボタンでスキップ
                            IsRButtonPressedRet = true;
                            return IsRButtonPressedRet;
                        }
                    }
                }
            }

            return IsRButtonPressedRet;
        }


        // Telopコマンド用描画ルーチン
        public static void DisplayTelop(ref string msg)
        {
            Load(My.MyProject.Forms.frmTelop);
            {
                var withBlock = My.MyProject.Forms.frmTelop;
                Expression.FormatMessage(ref msg);
                if (Strings.InStr(msg, ".") > 0)
                {
                    StringType.MidStmtStr(ref msg, Strings.InStr(msg, "."), Constants.vbCr.Length, Constants.vbCr);
                    withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(1170d);
                }
                else
                {
                    withBlock.Height = (int)SrcFormatter.TwipsToPixelsY(800d);
                }

                if (MainForm.Visible == true & !((int)MainForm.WindowState == 1))
                {
                    withBlock.Left = (int)SrcFormatter.TwipsToPixelsX(SrcFormatter.PixelsToTwipsX((double)MainForm.Left) + (MainForm.picMain(0).width * SrcFormatter.PixelsToTwipsX((double)MainForm.Width) / SrcFormatter.PixelsToTwipsX((double)MainForm.ClientRectangle.Width) - SrcFormatter.PixelsToTwipsX((double)withBlock.Width)) / 2);
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(MainForm.Top) + (long)(SrcFormatter.PixelsToTwipsY(MainForm.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2L);
                }
                else
                {
                    withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
                    withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);
                }

                if (Strings.InStr(msg, ".") > 0)
                {
                    StringType.MidStmtStr(ref msg, Strings.InStr(msg, "."), Constants.vbCr.Length, Constants.vbCr);
                }

                withBlock.Label1.Text = msg;
                withBlock.Show();
                withBlock.Refresh();
            }

            if ((GetAsyncKeyState(RButtonID) & 0x8000) == 0)
            {
                Sleep(1000);
            }

            My.MyProject.Forms.frmTelop.Close();
        }
    }
}