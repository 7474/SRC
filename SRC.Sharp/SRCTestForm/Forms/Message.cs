// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。zzzzz
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using SRC.Core;

namespace SRCTestForm
{
    // メッセージウィンドウのフォーム
    internal partial class frmMessage : Form
    {
        public SRC.Core.SRC SRC { get; set; }
        public IGUI GUI => SRC.GUI;

        public void ClearForm()
        {
            picFace.Image = null;
            picMessage.Image = null;
        }

        public void SetMessage(string message)
        {
            labKariText.Text = message;
        }

        // フォーム上をクリック
        private void frmMessage_Click(object eventSender, EventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
        }

        // フォーム上をダブルクリック
        private void frmMessage_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
        }

        // フォーム上でキーを押す
        private void frmMessage_KeyDown(object eventSender, KeyEventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
        }

        // フォーム上でマウスボタンを押す
        private void frmMessage_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
        }

        private void frmMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            // SRCを終了するか確認
            var ret = Interaction.MsgBox("SRCを終了しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "終了");
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

        // パイロット画面上でクリック
        private void picFace_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            // 自動メッセージ送りモードに移行
            if (GUI.MessageWait < 10000)
            {
                GUI.AutoMessageMode = !GUI.AutoMessageMode;
            }

            GUI.IsFormClicked = true;
        }

        // メッセージ欄上でダブルクリック
        private void picMessage_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
        }

        // メッセージ欄上でマウスボタンを押す
        private void picMessage_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
        }

        private void labKariText_DoubleClick(object sender, EventArgs e)
        {
            GUI.IsFormClicked = true;
        }

        private void labKariText_MouseDown(object sender, MouseEventArgs eventArgs)
        {
            GUI.IsFormClicked = true;
        }

    }
}