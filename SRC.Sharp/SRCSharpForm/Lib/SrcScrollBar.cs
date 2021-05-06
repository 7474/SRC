using System;
using System.Windows.Forms;

namespace SRCSharpForm.Lib
{
    // 方向キーでのスクロールはスクロールバーでは処理したくない。
    // そのためキー入力だった場合はスクロール処理を発生させない。
    // 裏付けのない結果として狙った振る舞いを得られている状態のコード。

    public class SrcHScrollBar : HScrollBar
    {
        private bool isKey;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            isKey = true;
            return false;
        }
        protected override bool ProcessDialogChar(char charCode)
        {
            isKey = true;
            return false;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            isKey = true;
            return false;
        }
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            isKey = true;
            return false;
        }
        protected override bool ProcessKeyPreview(ref Message m)
        {
            isKey = true;
            return false;
        }
        protected override void OnValueChanged(EventArgs e)
        {
            if (!isKey)
            {
                base.OnValueChanged(e);
            }
            isKey = false;
        }
    }
    public class SrcVScrollBar : VScrollBar
    {
        private bool isKey;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            isKey = true;
            return false;
        }
        protected override bool ProcessDialogChar(char charCode)
        {
            isKey = true;
            return false;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            isKey = true;
            return false;
        }
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            isKey = true;
            return false;
        }
        protected override bool ProcessKeyPreview(ref Message m)
        {
            isKey = true;
            return false;
        }
        protected override void OnValueChanged(EventArgs e)
        {
            if (!isKey)
            {
                base.OnValueChanged(e);
            }
            isKey = false;
        }
    }
}
