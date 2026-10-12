using System;
using System.Runtime.InteropServices;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// OS のマウスカーソルを読み書きする。Avalonia にはカーソルを動かす API が無いため、OS の API を直接呼ぶ。
    /// 座標はスクリーンの物理ピクセル（macOS はポイント）。
    /// - Windows: user32 の GetCursorPos / SetCursorPos
    ///   https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-setcursorpos
    /// - macOS: CoreGraphics の CGEventGetLocation / CGWarpMouseCursorPosition
    ///   https://developer.apple.com/documentation/coregraphics/1456387-cgwarpmousecursorposition
    /// - Linux（X11 / XWayland）: libX11 の XQueryPointer / XWarpPointer
    ///   https://www.x.org/releases/current/doc/man/man3/XWarpPointer.3.xhtml
    /// Wayland のネイティブ環境ではアプリからカーソルを動かせないため、何もしない。
    /// </summary>
    public static class NativeCursor
    {
        public static bool TryGetPosition(out int x, out int y)
        {
            x = y = 0;
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    if (!Win32.GetCursorPos(out var p))
                    {
                        return false;
                    }
                    x = p.X;
                    y = p.Y;
                    return true;
                }
                if (OperatingSystem.IsMacOS())
                {
                    var ev = MacOS.CGEventCreate(IntPtr.Zero);
                    if (ev == IntPtr.Zero)
                    {
                        return false;
                    }
                    var p = MacOS.CGEventGetLocation(ev);
                    MacOS.CFRelease(ev);
                    x = (int)Math.Round(p.X);
                    y = (int)Math.Round(p.Y);
                    return true;
                }
                if (OperatingSystem.IsLinux())
                {
                    return X11.TryQuery(out x, out y);
                }
            }
            catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
            {
            }
            return false;
        }

        public static bool SetPosition(int x, int y)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    return Win32.SetCursorPos(x, y);
                }
                if (OperatingSystem.IsMacOS())
                {
                    if (MacOS.CGWarpMouseCursorPosition(new MacOS.CGPoint { X = x, Y = y }) != 0)
                    {
                        return false;
                    }
                    // 移動直後にマウスの動きが一時的に止まらないようにする
                    MacOS.CGAssociateMouseAndMouseCursorPosition(1);
                    return true;
                }
                if (OperatingSystem.IsLinux())
                {
                    return X11.Warp(x, y);
                }
            }
            catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
            {
            }
            return false;
        }

        private static class Win32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;
            }

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetCursorPos(out POINT point);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetCursorPos(int x, int y);
        }

        private static class MacOS
        {
            private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";
            private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

            [StructLayout(LayoutKind.Sequential)]
            public struct CGPoint
            {
                public double X;
                public double Y;
            }

            [DllImport(CoreGraphics)]
            public static extern int CGWarpMouseCursorPosition(CGPoint point);

            [DllImport(CoreGraphics)]
            public static extern int CGAssociateMouseAndMouseCursorPosition(int connected);

            [DllImport(CoreGraphics)]
            public static extern IntPtr CGEventCreate(IntPtr source);

            [DllImport(CoreGraphics)]
            public static extern CGPoint CGEventGetLocation(IntPtr ev);

            [DllImport(CoreFoundation)]
            public static extern void CFRelease(IntPtr obj);
        }

        private static class X11
        {
            private const string LibX11 = "libX11.so.6";
            private static readonly object sync = new object();
            private static IntPtr display;
            private static bool unavailable;

            [DllImport(LibX11)]
            private static extern IntPtr XOpenDisplay(IntPtr name);

            [DllImport(LibX11)]
            private static extern IntPtr XDefaultRootWindow(IntPtr display);

            [DllImport(LibX11)]
            private static extern int XWarpPointer(IntPtr display, IntPtr srcWindow, IntPtr destWindow,
                int srcX, int srcY, uint srcWidth, uint srcHeight, int destX, int destY);

            [DllImport(LibX11)]
            private static extern int XFlush(IntPtr display);

            [DllImport(LibX11)]
            private static extern int XQueryPointer(IntPtr display, IntPtr window, out IntPtr root, out IntPtr child,
                out int rootX, out int rootY, out int winX, out int winY, out uint mask);

            private static IntPtr Display()
            {
                if (display == IntPtr.Zero && !unavailable)
                {
                    // DISPLAY が無い（Wayland ネイティブ・ヘッドレス）環境では使わない
                    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
                    {
                        unavailable = true;
                        return IntPtr.Zero;
                    }
                    display = XOpenDisplay(IntPtr.Zero);
                    unavailable = display == IntPtr.Zero;
                }
                return display;
            }

            public static bool TryQuery(out int x, out int y)
            {
                x = y = 0;
                lock (sync)
                {
                    var d = Display();
                    if (d == IntPtr.Zero)
                    {
                        return false;
                    }
                    return XQueryPointer(d, XDefaultRootWindow(d), out _, out _, out x, out y, out _, out _, out _) != 0;
                }
            }

            public static bool Warp(int x, int y)
            {
                lock (sync)
                {
                    var d = Display();
                    if (d == IntPtr.Zero)
                    {
                        return false;
                    }
                    XWarpPointer(d, IntPtr.Zero, XDefaultRootWindow(d), 0, 0, 0, 0, x, y);
                    XFlush(d);
                    return true;
                }
            }
        }
    }
}
