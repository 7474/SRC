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

    public class SrcVScrollBar : VScrollBar
    {
        private bool onMouseProc;
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