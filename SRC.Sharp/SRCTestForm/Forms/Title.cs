// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using System;
using System.Reflection;
using System.Windows.Forms;

namespace SRCTestForm
{
    // タイトル画面用フォーム
    internal partial class frmTitle : Form
    {
        private void frmTitle_Load(object eventSender, EventArgs eventArgs)
        {
            labVersion.Text = "Ver " + Assembly.GetEntryAssembly().GetName().Version.ToString();
        }
    }
}