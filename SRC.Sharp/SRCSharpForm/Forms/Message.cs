// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Microsoft.Extensions.Logging;
using SRCCore;
using System;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // メッセージウィンドウのフォーム
    internal partial class frmMessage : Form
    {
        public SRCCore.SRC SRC { get; set; }
        public IGUI GUI => SRC.GUI;

        public void ClearForm()
        {
            picFace.Image = null;
            picMessage.Image = null;
        }

        public void SetMessageModeCaption(bool isAuto)
        {
            if (isAuto)
            {
                Text = "メッセージ (自動送り)";
            }
            else
            {
                Text = "メッセージ";
            }
        }

        // フォーム上をクリック
        private void frmMessage_Click(object eventSender, EventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
            Program.Log.LogDebug("frmMessage_Click");
        }

        // フォーム上をダブルクリック
        private void frmMessage_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
            Program.Log.LogDebug("frmMessage_DoubleClick");
        }

        // フォーム上でキーを押す
        private void frmMessage_KeyDown(object eventSender, KeyEventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
            Program.Log.LogDebug("frmMessage_KeyDown");
        }

        // パイロット画面上でクリック
        private void picFace_Click(object eventSender, EventArgs eventArgs)
        {
            // 自動メッセージ送りモードに移行
            if (GUI.MessageWait < 10000)
            {
                GUI.AutoMessageMode = !GUI.AutoMessageMode;
            }

            GUI.IsFormClicked = true;
            Program.Log.LogDebug("picFace_Click");
        }

        // メッセージ欄上でクリック
        private void picMessage_Click(object sender, EventArgs e)
        {
            GUI.IsFormClicked = true;
            Program.Log.LogDebug("picMessage_Click");
        }

        // メッセージ欄上でダブルクリック
        private void picMessage_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
            Program.Log.LogDebug("picMessage_DoubleClick");
        }

        private void labKariText_Click(object sender, EventArgs e)
        {
            GUI.IsFormClicked = true;
            Program.Log.LogDebug("labKariText_Click");
        }

        private void labKariText_DoubleClick(object sender, EventArgs e)
        {
            GUI.IsFormClicked = true;
            Program.Log.LogDebug("labKariText_DoubleClick");
        }

        private void frmMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            // SRCを終了するか確認
            var ret = Interaction.MsgBox("SRCを終了しますか？", MsgBoxStyle.OkCancel | MsgBoxStyle.Question, "終了");
            switch (ret)
            {
                case MsgBoxResult.Ok:
                    // SRCを終了
                    Hide();
                    SRC.TerminateSRC();
                    break;

                default:
                    // 終了をキャンセル
                    e.Cancel = true;
                    break;
            }
        }
    }
}
