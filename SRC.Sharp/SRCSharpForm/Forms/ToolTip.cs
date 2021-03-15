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
            ret = GUI.SetWindowPos(Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
        }

        // ツールチップを表示
        public void ShowToolTip(ref string msg)
        {
            int ret;
            var PT = default(GUI.POINTAPI);
            short tw;
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                    Static cur_msg As String

             */
            tw = (short)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX();
            if ((msg ?? "") != (cur_msg ?? ""))
            {
                cur_msg = msg;
                {
                    var withBlock = picMessage;
                    // メッセージ長にサイズを合わせる
                    // UPGRADE_ISSUE: PictureBox メソッド picMessage.TextWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    withBlock.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX((withBlock.TextWidth(msg) + 6) * tw);
                    // UPGRADE_ISSUE: PictureBox メソッド picMessage.TextHeight はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    withBlock.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((withBlock.TextHeight(msg) + 4) * tw);
                    Width = withBlock.Width;
                    Height = withBlock.Height;

                    // UPGRADE_ISSUE: PictureBox メソッド picMessage.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    withBlock.Cls();
                    withBlock.ForeColor = ColorTranslator.FromOle(Information.RGB(200, 200, 200));
                    // UPGRADE_ISSUE: PictureBox メソッド picMessage.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    My.MyProject.Forms.frmToolTip.picMessage.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    // UPGRADE_ISSUE: PictureBox メソッド picMessage.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    My.MyProject.Forms.frmToolTip.picMessage.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    withBlock.ForeColor = Color.Black;
                    // UPGRADE_ISSUE: PictureBox メソッド picMessage.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    My.MyProject.Forms.frmToolTip.picMessage.Line(0, (long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Height) - 1d) / tw); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    // UPGRADE_ISSUE: PictureBox メソッド picMessage.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    My.MyProject.Forms.frmToolTip.picMessage.Line((long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Width) - 1d) / tw, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */

                    // メッセージを書き込み
                    // UPGRADE_ISSUE: PictureBox プロパティ picMessage.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    withBlock.CurrentX = 3;
                    // UPGRADE_ISSUE: PictureBox プロパティ picMessage.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    withBlock.CurrentY = 2;
                    // UPGRADE_ISSUE: PictureBox メソッド picMessage.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    My.MyProject.Forms.frmToolTip.picMessage.Print(msg);
                    withBlock.ForeColor = Color.White;
                    withBlock.Refresh();
                }
            }

            // フォームの位置を設定
            ret = GUI.GetCursorPos(ref PT);
            Left = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(PT.X * tw + 0);
            Top = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((PT.Y + 24) * tw);

            // フォームを非アクティブで表示
            ret = GUI.ShowWindow(Handle.ToInt32(), GUI.SW_SHOWNA);
        }
    }
}