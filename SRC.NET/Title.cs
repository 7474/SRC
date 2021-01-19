using System;
using System.Windows.Forms;

namespace Project1
{
    internal partial class frmTitle : Form
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // タイトル画面用フォーム

        private void frmTitle_Load(object eventSender, EventArgs eventArgs)
        {
            // UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
            {
                var withBlock = App;
                labVersion.Text = "Ver " + My.MyProject.Application.Info.Version.Major + "." + My.MyProject.Application.Info.Version.Minor + "." + My.MyProject.Application.Info.Version.Revision + "a";
            }
        }
    }
}