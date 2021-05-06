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
        protected override void WndProc(ref Message m)
        {
            if (!isScrollMsg(m))
            {
                base.WndProc(ref m);
            }
        }
        private static bool isScrollMsg(Message msg)
        {
            //    case NativeMethods.WM_REFLECT + NativeMethods.WM_HSCROLL:
            //    case NativeMethods.WM_REFLECT + NativeMethods.WM_VSCROLL:
            //        WmReflectScroll(ref m);
            //    WM_HSCROLL = 0x0114,
            //    WM_VSCROLL = 0x0115,
            //    WM_USER = 0x0400,
            //    WM_REFLECT = NativeMethods.WM_USER + 0x1C00,
            return msg.Msg == 0x0400 + 0x1c00 + 0x0114
                || msg.Msg == 0x0400 + 0x1c00 + 0x0115;
        }
    }

    public class SrcVScrollBar : VScrollBar
    {
        protected override void WndProc(ref Message m)
        {
            if (!isScrollMsg(m))
            {
                base.WndProc(ref m);
            }
        }
        private static bool isScrollMsg(Message msg)
        {
            //    case NativeMethods.WM_REFLECT + NativeMethods.WM_HSCROLL:
            //    case NativeMethods.WM_REFLECT + NativeMethods.WM_VSCROLL:
            //        WmReflectScroll(ref m);
            //    WM_HSCROLL = 0x0114,
            //    WM_VSCROLL = 0x0115,
            //    WM_USER = 0x0400,
            //    WM_REFLECT = NativeMethods.WM_USER + 0x1C00,
            return msg.Msg == 0x0400 + 0x1c00 + 0x0114
                || msg.Msg == 0x0400 + 0x1c00 + 0x0115;
        }
    }
}