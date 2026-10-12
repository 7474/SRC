using SRCCore;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCRender
{
    public partial class ScreenGUI
    {
        public bool MessageFormVisible => State.Message.Visible;

        public void OpenMessageForm(Unit u1 = null, Unit u2 = null)
        {
            State.Message.Visible = true;
            State.Message.Lines.Clear();
            State.Message.Speaker = null;
            Present();
        }

        public void CloseMessageForm()
        {
            State.Message.Visible = false;
            State.Message.Waiting = false;
            Present();
        }

        public void ClearMessageForm()
        {
            State.Message.Lines.Clear();
            State.Message.Speaker = null;
            Present();
        }

        public void UpdateMessageForm(Unit u1, Unit u2 = null)
        {
        }

        public void SaveMessageFormStatus()
        {
        }

        public void KeepMessageFormStatus()
        {
        }

        public void DisplayMessage(string pname, string msg, string msg_mode = "")
        {
            src.Expression.FormatMessage(ref msg);
            msg = Strings.Trim(msg ?? "");
            while (msg.EndsWith(";"))
            {
                msg = msg.Substring(0, msg.Length - 1);
            }
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            State.Message.Visible = true;
            State.Message.Speaker = pname == "システム" ? null : ResolveSpeaker(pname);
            State.Message.Lines.Clear();
            foreach (var line in renderer.WrapText(msg.Replace(";", Environment.NewLine), ScreenLayout.Width - 64))
            {
                State.Message.Lines.Add(line);
            }
            WaitMessage(State.Message.Lines.Count);
        }

        private string ResolveSpeaker(string pname)
        {
            if (string.IsNullOrEmpty(pname))
            {
                return null;
            }
            try
            {
                if (src.PList.IsDefined(pname))
                {
                    return src.PList.Item(pname).get_Nickname(false);
                }
                if (src.NPDList.IsDefined(pname))
                {
                    return src.NPDList.Item(pname).Nickname;
                }
            }
            catch
            {
                // 名前の解決に失敗した場合はそのまま表示する
            }
            return pname;
        }

        /// <summary>
        /// クリック、右ボタン、自動送りのいずれかまで待つ。
        /// </summary>
        private void WaitMessage(int lines)
        {
            if (MessageWait > 10000)
            {
                AutoMessageMode = false;
            }
            // 待ちに入る前のクリックは無効にする（先行入力の解消）
            IsFormClicked = false;
            State.Message.AutoMode = AutoMessageMode;
            State.Message.Waiting = true;
            Present();

            var waitMillis = (lines + 2) * (MessageWait + 250);
            WaitUntil(() => IsFormClicked || IsRButtonPressed(true), AutoMessageMode ? waitMillis : -1);
            IsFormClicked = false;

            State.Message.Waiting = false;
            Present();
        }

        public void PrintMessage(string msg, bool is_sys_msg = false)
        {
            src.Expression.FormatMessage(ref msg);
            foreach (var line in renderer.WrapText(msg ?? "", ScreenLayout.Width - 64))
            {
                State.Message.Lines.Add(line);
            }
            Present();
        }

        public void DisplayBattleMessage(string pname, string msg, string msg_mode = "")
        {
            src.Expression.FormatMessage(ref msg);
            State.Message.Visible = true;
            State.Message.Speaker = ResolveSpeaker(pname);
            State.Message.Lines.Clear();
            foreach (var line in renderer.WrapText(msg ?? "", ScreenLayout.Width - 64))
            {
                State.Message.Lines.Add(line);
            }
            Present();
            IsFormClicked = false;
            WaitUntil(() => IsFormClicked || IsRButtonPressed(), Math.Max(MessageWait, 0) + 300);
            IsFormClicked = false;
        }

        public void DisplaySysMessage(string msg, bool int_wait = false)
        {
            src.Expression.FormatMessage(ref msg);
            State.Message.Visible = true;
            State.Message.Speaker = null;
            State.Message.Lines.Clear();
            foreach (var line in renderer.WrapText(msg ?? "", ScreenLayout.Width - 64))
            {
                State.Message.Lines.Add(line);
            }
            Present();
            if (int_wait)
            {
                IsFormClicked = false;
                WaitUntil(() => IsFormClicked || IsRButtonPressed(), Math.Max(MessageWait, 0) + 300);
                IsFormClicked = false;
            }
        }
    }
}
