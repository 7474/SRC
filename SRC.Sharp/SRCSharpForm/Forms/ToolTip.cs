using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Project1
{
    internal partial class frmToolTip : Form
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // ツールチップ用フォーム

        // フォームをロード
        private void frmToolTip_Load(object eventSender, EventArgs eventArgs)
        {
            int ret;

            // 常に手前に表示
            //ret = GUI.SetWindowPos(Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
        }

        // ツールチップを表示
        public void ShowToolTip(ref string msg)
        {
            //int ret;
            //var PT = default(GUI.POINTAPI);
            //short tw;
            //;
            ///* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
            //        Static cur_msg As String

            // */
            //tw = (short)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX();
            //if ((msg ?? "") != (cur_msg ?? ""))
            //{
            //    cur_msg = msg;
            //    {
            //        var withBlock = picMessage;
            //        // メッセージ長にサイズを合わせる
            //        withBlock.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX((withBlock.TextWidth(msg) + 6) * tw);
            //        withBlock.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((withBlock.TextHeight(msg) + 4) * tw);
            //        Width = withBlock.Width;
            //        Height = withBlock.Height;

            //        withBlock.Cls();
            //        withBlock.ForeColor = ColorTranslator.FromOle(Information.RGB(200, 200, 200));
            //        My.MyProject.Forms.frmToolTip.picMessage.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        My.MyProject.Forms.frmToolTip.picMessage.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        withBlock.ForeColor = Color.Black;
            //        My.MyProject.Forms.frmToolTip.picMessage.Line(0, (long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Height) - 1d) / tw); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        My.MyProject.Forms.frmToolTip.picMessage.Line((long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Width) - 1d) / tw, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */

            //        // メッセージを書き込み
            //        withBlock.CurrentX = 3;
            //        withBlock.CurrentY = 2;
            //        My.MyProject.Forms.frmToolTip.picMessage.Print(msg);
            //        withBlock.ForeColor = Color.White;
            //        withBlock.Refresh();
            //    }
            //}

            //// フォームの位置を設定
            //ret = GUI.GetCursorPos(ref PT);
            //Left = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(PT.X * tw + 0);
            //Top = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((PT.Y + 24) * tw);

            //// フォームを非アクティブで表示
            //ret = GUI.ShowWindow(Handle.ToInt32(), GUI.SW_SHOWNA);
        }
    }
}