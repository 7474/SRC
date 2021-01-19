using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Project1
{
    static class Flash
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // Flashファイルの再生
        public static void PlayFlash(ref string fname, ref short fx, ref short fy, ref short fw, ref short fh, ref string opt)
        {
            short i;
            bool is_VisibleEnd;

            // FLASHが使用できない場合はエラー
            if (!GUI.IsFlashAvailable)
            {
                string argmsg = "Flashファイルの読み込み中にエラーが発生しました。" + Constants.vbCrLf + "「Macromedia Flash Player」がインストールされていません。" + Constants.vbCrLf + "次のURLから、最新版のFlash Playerをインストールしてください。" + Constants.vbCrLf + "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese";
                GUI.ErrorMessage(ref argmsg);
                return;
            }
            // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Enable の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            if (!My.MyProject.Forms.frmMain.FlashObject.Enable)
            {
                string argmsg1 = "Flashファイルの読み込み中にエラーが発生しました。" + Constants.vbCrLf + "「Macromedia Flash Player」がインストールされていません。" + Constants.vbCrLf + "次のURLから、最新版のFlash Playerをインストールしてください。" + Constants.vbCrLf + "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese";
                GUI.ErrorMessage(ref argmsg1);
                return;
            }

            is_VisibleEnd = false;
            var loopTo = GeneralLib.LLength(ref opt);
            for (i = 1; i <= loopTo; i++)
            {
                switch (GeneralLib.LIndex(ref opt, i) ?? "")
                {
                    case "保持":
                        {
                            is_VisibleEnd = true;
                            break;
                        }
                }
            }

            {
                var withBlock = My.MyProject.Forms.frmMain.FlashObject;
                // 一旦非表示
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Visible の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.Visible = false;

                // Flashオブジェクトの位置・サイズ設定
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Left の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.Left = fx;
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Top の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.Top = fy;
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Width の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.Width = fw;
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Height の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.Height = fh;
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Visible の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.Visible = true;
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.ZOrder の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.ZOrder();

                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.LoadMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.LoadMovie(SRC.ScenarioPath + fname);

                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Playing の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                while (withBlock.Playing & !GUI.IsRButtonPressed(true))
                    Application.DoEvents();

                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.StopMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                My.MyProject.Forms.frmMain.FlashObject.StopMovie();
                if (!is_VisibleEnd)
                {
                    // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.ClearMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock.ClearMovie();
                    // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Visible の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock.Visible = false;
                }
            }
        }

        // 表示したままのFlashを消去する
        public static void ClearFlash()
        {
            if (!GUI.IsFlashAvailable)
                return;
            // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Enable の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            if (!My.MyProject.Forms.frmMain.FlashObject.Enable)
                return;
            {
                var withBlock = My.MyProject.Forms.frmMain.FlashObject;
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.ClearMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.ClearMovie();
                // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Visible の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                withBlock.Visible = false;
            }
        }

        // Flashファイルからイベントを取得
        // Flashのアクションの「GetURL」で
        // 1.「URL」に"FSCommand:"
        // 2.「ターゲット」に「サブルーチン名 [引数1 [引数2 […]]」
        // を指定すると、そのアクションが実行されたときに
        // ターゲットのサブルーチンが実行される。
        // サブルーチンを実行している間、Flashの再生は停止する。
        public static void GetEvent(string fpara)
        {
            var buf = default(string);
            short i, j;
            string funcname, funcpara;
            var etype = default(Expression.ValueType);
            var str_result = default(string);
            var num_result = default(double);

            // 再生を一時停止
            // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.StopMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            My.MyProject.Forms.frmMain.FlashObject.StopMovie();
            funcname = "";
            funcpara = "";

            // 念のためにFlashから渡されたパラメータ全てを解析
            // 一番最初に見つかった文字列を、呼び出すサブルーチン名とする
            if (string.IsNullOrEmpty(funcname))
            {
                funcname = GeneralLib.ListIndex(ref fpara, 1);
                buf = GeneralLib.ListTail(ref fpara, 2);
            }
            // サブルーチンの引数を記録
            var loopTo = GeneralLib.ListLength(ref buf);
            for (j = 1; j <= loopTo; j++)
                funcpara = funcpara + ", " + GeneralLib.ListIndex(ref buf, j);

            // サブルーチン名と引数から、Call関数の呼び出しの文字列を生成
            buf = "Call(" + funcname + funcpara + ")";
            // 式として生成した文字列を実行
            Expression.CallFunction(ref buf, ref etype, ref str_result, ref num_result);

            // 再生を再開
            // UPGRADE_WARNING: オブジェクト frmMain.FlashObject.PlayMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            My.MyProject.Forms.frmMain.FlashObject.PlayMovie();
        }
    }
}