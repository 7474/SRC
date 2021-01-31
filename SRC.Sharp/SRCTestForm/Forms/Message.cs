// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using System;
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
            short KeyCode = (short)eventArgs.KeyCode;
            short Shift = (short)((int)eventArgs.KeyData / 0x10000);
            GUI.IsFormClicked = true;
        }

        // フォーム上でマウスボタンを押す
        private void frmMessage_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            short Button = (short)((int)eventArgs.Button / 0x100000);
            short Shift = (short)((int)ModifierKeys / 0x10000);
            float X = eventArgs.X;
            float Y = eventArgs.Y;
            GUI.IsFormClicked = true;
        }

        // フォームを閉じる
        private void frmMessage_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
        {
            short ret;

            // SRCを終了するか確認
            ret = (short)Interaction.MsgBox("SRCを終了しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "終了");
            switch (ret)
            {
                case 1:
                    {
                        // SRCを終了
                        Hide();
                        SRC.TerminateSRC();
                        break;
                    }

                case 2:
                    {
                        // 終了をキャンセル
                        // UPGRADE_ISSUE: Event パラメータ Cancel はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' をクリックしてください。
                        //Cancel = 1;
                        break;
                    }
            }
        }

        // パイロット画面上でクリック
        private void picFace_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            short Button = (short)((int)eventArgs.Button / 0x100000);
            short Shift = (short)((int)ModifierKeys / 0x10000);
            float X = eventArgs.X;
            float Y = eventArgs.Y;
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
            short Button = (short)((int)eventArgs.Button / 0x100000);
            short Shift = (short)((int)ModifierKeys / 0x10000);
            float X = eventArgs.X;
            float Y = eventArgs.Y;
            GUI.IsFormClicked = true;
        }
    }
}