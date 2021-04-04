// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using System;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // データロードの進行状況を示すフォーム
    internal partial class frmNowLoading : Form
    {
        // データ総数
        public int Max { get { return progressBar.Maximum; } set { progressBar.Maximum = value; } }
        // 読み込み終えたデータの数
        public int Value { get { return progressBar.Value; } set { progressBar.Value = value; } }

        // ロードを１段階進行させる
        public void Progress()
        {
            Value = Math.Min(Value + 1, Max);
            Refresh();
        }
    }
}
