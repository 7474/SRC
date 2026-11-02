using System;
using System.Runtime.InteropServices;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// OS のメニュー・画面の文字（フォントの名前と大きさ。大きさは論理ピクセル）。メニューやダイアログの文字を OS の表示に合わせるのに使う。
    /// </summary>
    internal sealed class SystemUiFont
    {
        public SystemUiFont(string familyName, double size)
        {
            FamilyName = familyName;
            Size = size;
        }

        /// <summary>フォントの名前（null なら Avalonia の既定）</summary>
        public string FamilyName { get; }

        /// <summary>文字の大きさ（論理ピクセル）</summary>
        public double Size { get; }

        /// <summary>
        /// OS の文字を調べる。Windows はメニューのフォント（SPI_GETNONCLIENTMETRICS の lfMenuFont。既定は 9 ポイント＝12 ピクセル）、
        /// macOS はシステムの文字の標準（13 ポイント）。分からなければ null（テーマの既定のまま）
        /// </summary>
        public static SystemUiFont Detect()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    return WindowsMenuFont();
                }
                if (OperatingSystem.IsMacOS())
                {
                    // https://developer.apple.com/design/human-interface-guidelines/typography （本文・メニュー 13 pt）
                    return new SystemUiFont(null, 13);
                }
            }
            catch (Exception)
            {
                // 調べられなければテーマの既定のまま
            }
            return null;
        }

        private const uint SPI_GETNONCLIENTMETRICS = 0x0029;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct LOGFONTW
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NONCLIENTMETRICSW
        {
            public int cbSize;
            public int iBorderWidth;
            public int iScrollWidth;
            public int iScrollHeight;
            public int iCaptionWidth;
            public int iCaptionHeight;
            public LOGFONTW lfCaptionFont;
            public int iSmCaptionWidth;
            public int iSmCaptionHeight;
            public LOGFONTW lfSmCaptionFont;
            public int iMenuWidth;
            public int iMenuHeight;
            public LOGFONTW lfMenuFont;
            public LOGFONTW lfStatusFont;
            public LOGFONTW lfMessageFont;
            public int iPaddedBorderWidth;
        }

        // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-systemparametersinfofordpi
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SystemParametersInfoForDpi(uint uiAction, uint uiParam, ref NONCLIENTMETRICSW pvParam, uint fWinIni, uint dpi);

        private static SystemUiFont WindowsMenuFont()
        {
            var metrics = new NONCLIENTMETRICSW { cbSize = Marshal.SizeOf<NONCLIENTMETRICSW>() };
            // 96 DPI での値（＝論理ピクセル）を求める。Avalonia が画面の倍率を掛ける
            if (!SystemParametersInfoForDpi(SPI_GETNONCLIENTMETRICS, (uint)metrics.cbSize, ref metrics, 0, 96))
            {
                return null;
            }
            var height = Math.Abs(metrics.lfMenuFont.lfHeight);
            if (height == 0)
            {
                return null;
            }
            var name = string.IsNullOrWhiteSpace(metrics.lfMenuFont.lfFaceName) ? null : metrics.lfMenuFont.lfFaceName;
            return new SystemUiFont(name, height);
        }
    }
}
