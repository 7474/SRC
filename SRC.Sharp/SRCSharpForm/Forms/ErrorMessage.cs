using System.Windows.Forms;

namespace SRCSharpForm
{
    internal partial class frmErrorMessage : Form
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // エラーメッセージ表示用フォーム

        public void SetErrorMessage(string msg)
        {
            txtMessage.Text = msg;
        }
    }
}
