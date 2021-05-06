using System.Diagnostics;
using System.Windows.Forms;

namespace SRCSharpForm.Lib
{
    // 方向キーでのスクロールはスクロールバーでは処理したくない。
    // そのためキー入力に伴うスクロールメッセージは処理しない。
    // マウスホイールに伴うスクロールは、マウス操作メッセージの処理中で行っている。
    // 値の直接設定後はスクロールメッセージではない。
    // https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/ScrollBar.cs

    public class SrcHScrollBar : HScrollBar
    {
        private bool onMouseProc;
        //}
        protected override void WndProc(ref Message m)
        {
            onMouseProc |= isMouseDown(m);
            onMouseProc &= !isMouseUp(m);

            if (onMouseProc || !isScrollMsg(m))
            {
                base.WndProc(ref m);
            }
            //else
            //{
            //    Debug.WriteLine(m);
            //}
        }
        //WM_MOUSEFIRST = 0x0200,
        //WM_MOUSEMOVE = 0x0200,
        //WM_LBUTTONDOWN = 0x0201,
        //WM_LBUTTONUP = 0x0202,
        //WM_LBUTTONDBLCLK = 0x0203,
        //WM_RBUTTONDOWN = 0x0204,
        //WM_RBUTTONUP = 0x0205,
        //WM_RBUTTONDBLCLK = 0x0206,
        //WM_MBUTTONDOWN = 0x0207,
        //WM_MBUTTONUP = 0x0208,
        //WM_MBUTTONDBLCLK = 0x0209,
        //WM_XBUTTONDOWN = 0x020B,
        //WM_XBUTTONUP = 0x020C,
        //WM_XBUTTONDBLCLK = 0x020D,
        //WM_MOUSEWHEEL = 0x020A,
        //WM_MOUSELAST = 0x020A
        private static bool isMouseDown(Message msg)
        {
            return msg.Msg == 0x0201;
        }
        private static bool isMouseUp(Message msg)
        {
            return msg.Msg == 0x0202;
        }
        // {msg=0x2115 (WM_REFLECT + WM_VSCROLL) hwnd=0x510058 wparam=0x3 lparam=0x510058 result=0x0}
        // {msg=0x2115 (WM_REFLECT + WM_VSCROLL) hwnd=0x510058 wparam=0x8 lparam=0x510058 result=0x0}
        //    case NativeMethods.WM_REFLECT + NativeMethods.WM_HSCROLL:
        //    case NativeMethods.WM_REFLECT + NativeMethods.WM_VSCROLL:
        //        WmReflectScroll(ref m);
        //    WM_HSCROLL = 0x0114,
        //    WM_VSCROLL = 0x0115,
        //    WM_USER = 0x0400,
        //    WM_REFLECT = NativeMethods.WM_USER + 0x1C00,
        private static bool isScrollMsg(Message msg)
        {
            return msg.Msg == 0x0400 + 0x1c00 + 0x0114
                || msg.Msg == 0x0400 + 0x1c00 + 0x0115;
        }
    }

    public class SrcVScrollBar : VScrollBar
    {
        private bool onMouseProc;
        //}
        protected override void WndProc(ref Message m)
        {
            onMouseProc |= isMouseDown(m);
            onMouseProc &= !isMouseUp(m);

            if (onMouseProc || !isScrollMsg(m))
            {
                base.WndProc(ref m);
            }
        }
        private static bool isMouseDown(Message msg)
        {
            return msg.Msg == 0x0201;
        }
        private static bool isMouseUp(Message msg)
        {
            return msg.Msg == 0x0202;
        }
        private static bool isScrollMsg(Message msg)
        {
            return msg.Msg == 0x0400 + 0x1c00 + 0x0114
                || msg.Msg == 0x0400 + 0x1c00 + 0x0115;
        }
    }
}