using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class FileDialog
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // APIだけでファイルダイアログを実現するためのモジュール

        // OPENFILENAME構造体
        private struct OPENFILENAME
        {
            public int lStructSize;
            public int hwndOwner;
            public int hInstance;
            public string lpstrFilter;
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int iFilterIndex;
            public string lpstrFile;
            public int nMaxFile;
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public int lCustData;
            public int lpfnHook;
            public string lpTemplateName;
        }

        // 「ファイルを開く」のダイアログを作成
        // UPGRADE_WARNING: 構造体 OPENFILENAME に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("comdlg32.dll", EntryPoint = "GetOpenFileNameA")]
        private static extern int GetOpenFileName(ref OPENFILENAME f);

        // 「ファイルを保存」のダイアログを作成
        // UPGRADE_WARNING: 構造体 OPENFILENAME に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("comdlg32.dll", EntryPoint = "GetSaveFileNameA")]
        private static extern int GetSaveFileName(ref OPENFILENAME f);

        // ファイルロード用のダイアログを表示するための関数
        public static string LoadFileDialog(ref string dtitle, ref string fpath, ref string default_file, short default_filter, ref string ftype, ref string fsuffix, [Optional, DefaultParameterValue("")] ref string ftype2, [Optional, DefaultParameterValue("")] ref string fsuffix2, [Optional, DefaultParameterValue("")] ref string ftype3, [Optional, DefaultParameterValue("")] ref string fsuffix3)
        {
            string LoadFileDialogRet = default;
            var f = default(OPENFILENAME);
            string fname, ftitle;
            int ret;
            fname = default_file + new string(Conversions.ToChar(Constants.vbNullChar), 1024 - Strings.Len(default_file));
            ftitle = Strings.Space(1024);

            // OPENFILENAME構造体の初期化
            {
                var withBlock = f;
                withBlock.hwndOwner = GUI.MainForm.Handle.ToInt32();
                if (!string.IsNullOrEmpty(ftype3))
                {
                    withBlock.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" + Constants.vbNullChar + "*.*" + Constants.vbNullChar + ftype + " (*." + fsuffix + ")" + Constants.vbNullChar + "*." + fsuffix + Constants.vbNullChar + ftype2 + " (*." + fsuffix2 + ")" + Constants.vbNullChar + "*." + fsuffix2 + Constants.vbNullChar + ftype3 + " (*." + fsuffix3 + ")" + Constants.vbNullChar + "*." + fsuffix3 + Constants.vbNullChar;
                }
                else if (!string.IsNullOrEmpty(ftype2))
                {
                    withBlock.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" + Constants.vbNullChar + "*.*" + Constants.vbNullChar + ftype + " (*." + fsuffix + ")" + Constants.vbNullChar + "*." + fsuffix + Constants.vbNullChar + ftype2 + " (*." + fsuffix2 + ")" + Constants.vbNullChar + "*." + fsuffix2 + Constants.vbNullChar;
                }
                else
                {
                    withBlock.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" + Constants.vbNullChar + "*.*" + Constants.vbNullChar + ftype + " (*." + fsuffix + ")" + Constants.vbNullChar + "*." + fsuffix + Constants.vbNullChar;
                }

                withBlock.iFilterIndex = default_filter;
                withBlock.lpstrFile = fname;
                withBlock.nMaxFile = 1024;
                withBlock.lpstrFileTitle = ftitle;
                withBlock.nMaxFileTitle = 1024;
                withBlock.lpstrInitialDir = fpath;
                withBlock.lpstrTitle = dtitle;
                withBlock.flags = 0x201804;
                withBlock.lpstrDefExt = "";
                withBlock.lStructSize = Strings.Len(f);
            }

            ret = GetOpenFileName(ref f);
            switch (ret)
            {
                case 0:
                    {
                        // キャンセルされた
                        LoadFileDialogRet = "";
                        break;
                    }

                case 1:
                    {
                        // ファイルを選択
                        LoadFileDialogRet = f.lpstrFile;
                        // vbNullChar までで切り出す。
                        LoadFileDialogRet = Strings.Left(LoadFileDialogRet, Strings.InStr(LoadFileDialogRet, Constants.vbNullChar) - 1);
                        break;
                    }
            }

            return LoadFileDialogRet;
        }

        // ファイルセーブ用のダイアログを表示するための関数
        public static string SaveFileDialog(ref string dtitle, ref string fpath, ref string default_file, short default_filter, ref string ftype, ref string fsuffix, [Optional, DefaultParameterValue("")] ref string ftype2, [Optional, DefaultParameterValue("")] ref string fsuffix2, [Optional, DefaultParameterValue("")] ref string ftype3, [Optional, DefaultParameterValue("")] ref string fsuffix3)
        {
            string SaveFileDialogRet = default;
            var f = default(OPENFILENAME);
            string fname, ftitle;
            int ret;
            fname = default_file + new string(Conversions.ToChar(Constants.vbNullChar), 1024 - Strings.Len(default_file));
            ftitle = Strings.Space(1024);

            // OPENFILENAME構造体の初期化
            {
                var withBlock = f;
                withBlock.hwndOwner = GUI.MainForm.Handle.ToInt32();
                if (!string.IsNullOrEmpty(ftype3))
                {
                    withBlock.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" + Constants.vbNullChar + "*.*" + Constants.vbNullChar + ftype + " (*." + fsuffix + ")" + Constants.vbNullChar + "*." + fsuffix + Constants.vbNullChar + ftype2 + " (*." + fsuffix2 + ")" + Constants.vbNullChar + "*." + fsuffix2 + Constants.vbNullChar + ftype3 + " (*." + fsuffix3 + ")" + Constants.vbNullChar + "*." + fsuffix2 + Constants.vbNullChar;
                }
                else if (!string.IsNullOrEmpty(ftype2))
                {
                    withBlock.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" + Constants.vbNullChar + "*.*" + Constants.vbNullChar + ftype + " (*." + fsuffix + ")" + Constants.vbNullChar + "*." + fsuffix + Constants.vbNullChar + ftype2 + " (*." + fsuffix2 + ")" + Constants.vbNullChar + "*." + fsuffix2 + Constants.vbNullChar;
                }
                else
                {
                    withBlock.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" + Constants.vbNullChar + "*.*" + Constants.vbNullChar + ftype + " (*." + fsuffix + ")" + Constants.vbNullChar + "*." + fsuffix + Constants.vbNullChar;
                }

                withBlock.iFilterIndex = default_filter;
                withBlock.lpstrFile = fname;
                withBlock.nMaxFile = 1024;
                withBlock.lpstrFileTitle = ftitle;
                withBlock.nMaxFileTitle = 1024;
                withBlock.lpstrInitialDir = fpath;
                withBlock.lpstrTitle = dtitle;
                withBlock.flags = 0x201804;
                withBlock.lpstrDefExt = "";
                withBlock.lStructSize = Strings.Len(f);
            }

            ret = GetSaveFileName(ref f);
            switch (ret)
            {
                case 0:
                    {
                        // キャンセルされた
                        SaveFileDialogRet = "";
                        break;
                    }

                case 1:
                    {
                        // ファイルを選択
                        SaveFileDialogRet = f.lpstrFile;
                        // vbNullChar までで切り出す。
                        SaveFileDialogRet = Strings.Left(SaveFileDialogRet, Strings.InStr(SaveFileDialogRet, Constants.vbNullChar) - 1);
                        break;
                    }
            }

            return SaveFileDialogRet;
        }
    }
}