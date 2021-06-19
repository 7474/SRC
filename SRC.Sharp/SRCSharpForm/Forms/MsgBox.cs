using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // 表示位置などを制御してメッセージボックスを表示するフォーム。
    // https://www.codeproject.com/Articles/17253/A-Custom-Message-Box
    // を参考にしました。
    public class MsgBox : Form
    {
        private static MsgBox _msgBox;
        private Panel _plHeader = new Panel();
        private Panel _plFooter = new Panel();
        private Panel _plIcon = new Panel();
        private PictureBox _picIcon = new PictureBox();
        private FlowLayoutPanel _flpButtons = new FlowLayoutPanel();
        private Label _lblMessage;
        private List<Button> _buttonCollection = new List<Button>();
        private static DialogResult _buttonResult = new DialogResult();

        private MsgBox()
        {
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new System.Windows.Forms.Padding(3);
            Width = 400;

            _lblMessage = new Label();
            _lblMessage.Dock = DockStyle.Fill;

            _flpButtons.FlowDirection = FlowDirection.RightToLeft;
            _flpButtons.Dock = DockStyle.Fill;

            _plHeader.Dock = DockStyle.Fill;
            _plHeader.Padding = new Padding(20);
            _plHeader.Controls.Add(_lblMessage);

            _plFooter.Dock = DockStyle.Bottom;
            _plFooter.Padding = new Padding(20);
            _plFooter.Height = 80;
            _plFooter.Controls.Add(_flpButtons);

            _picIcon.Width = 32;
            _picIcon.Height = 32;
            _picIcon.Location = new Point(30, 50);

            _plIcon.Dock = DockStyle.Left;
            _plIcon.Padding = new Padding(20);
            _plIcon.Width = 70;
            _plIcon.Controls.Add(_picIcon);

            Controls.Add(_plHeader);
            Controls.Add(_plIcon);
            Controls.Add(_plFooter);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MsgBox
            // 
            ClientSize = new System.Drawing.Size(284, 261);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MsgBox";
            ShowIcon = false;
            ShowInTaskbar = false;
            ResumeLayout(false);

        }

        private static void ShowMsgBox(IWin32Window owner)
        {
            _buttonResult = DialogResult.None;
            if (owner != null)
            {
                _msgBox.StartPosition = FormStartPosition.CenterParent;
                _msgBox.ShowDialog(owner);
            }
            else
            {
                _msgBox.StartPosition = FormStartPosition.CenterScreen;
                _msgBox.ShowDialog();
            }
        }

        public static void Show(IWin32Window owner, string message)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            ShowMsgBox(owner);
        }

        public static void Show(IWin32Window owner, string message, string title)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox.Text = title;
            _msgBox.Size = MessageSize(message);
            ShowMsgBox(owner);
        }

        public static DialogResult Show(IWin32Window owner, string message, string title, MsgBoxButtons buttons)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox.Text = title;
            _msgBox._plIcon.Hide();

            InitButtons(buttons);

            _msgBox.Size = MessageSize(message);
            ShowMsgBox(owner);
            return _buttonResult;
        }

        public static DialogResult Show(IWin32Window owner, string message, string title, MsgBoxButtons buttons, MsgBoxIcon icon)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox.Text = title;

            InitButtons(buttons);
            InitIcon(icon);

            _msgBox.Size = MessageSize(message);
            ShowMsgBox(owner);
            return _buttonResult;
        }

        private static void InitButtons(MsgBoxButtons buttons)
        {
            switch (buttons)
            {
                case MsgBoxButtons.AbortRetryIgnore:
                    _msgBox.InitAbortRetryIgnoreButtons();
                    break;

                case MsgBoxButtons.OK:
                    _msgBox.InitOKButton();
                    break;

                case MsgBoxButtons.OKCancel:
                    _msgBox.InitOKCancelButtons();
                    break;

                case MsgBoxButtons.RetryCancel:
                    _msgBox.InitRetryCancelButtons();
                    break;

                case MsgBoxButtons.YesNo:
                    _msgBox.InitYesNoButtons();
                    break;

                case MsgBoxButtons.YesNoCancel:
                    _msgBox.InitYesNoCancelButtons();
                    break;
            }

            foreach (Button btn in _msgBox._buttonCollection)
            {
                btn.Padding = new Padding(3);
                btn.Height = 30;

                _msgBox._flpButtons.Controls.Add(btn);
            }
        }

        private static void InitIcon(MsgBoxIcon icon)
        {
            switch (icon)
            {
                case MsgBoxIcon.Application:
                    _msgBox._picIcon.Image = SystemIcons.Application.ToBitmap();
                    break;

                case MsgBoxIcon.Exclamation:
                    _msgBox._picIcon.Image = SystemIcons.Exclamation.ToBitmap();
                    break;

                case MsgBoxIcon.Error:
                    _msgBox._picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case MsgBoxIcon.Info:
                    _msgBox._picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case MsgBoxIcon.Question:
                    _msgBox._picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                case MsgBoxIcon.Shield:
                    _msgBox._picIcon.Image = SystemIcons.Shield.ToBitmap();
                    break;

                case MsgBoxIcon.Warning:
                    _msgBox._picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
            }
        }

        private void InitAbortRetryIgnoreButtons()
        {
            Button btnAbort = new Button();
            btnAbort.Text = "Abort";
            btnAbort.Click += ButtonClick;

            Button btnRetry = new Button();
            btnRetry.Text = "Retry";
            btnRetry.Click += ButtonClick;

            Button btnIgnore = new Button();
            btnIgnore.Text = "Ignore";
            btnIgnore.Click += ButtonClick;

            _buttonCollection.Add(btnAbort);
            _buttonCollection.Add(btnRetry);
            _buttonCollection.Add(btnIgnore);
        }

        private void InitOKButton()
        {
            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Click += ButtonClick;

            _buttonCollection.Add(btnOK);
        }

        private void InitOKCancelButtons()
        {
            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "キャンセル";
            btnCancel.Click += ButtonClick;


            _buttonCollection.Add(btnOK);
            _buttonCollection.Add(btnCancel);
        }

        private void InitRetryCancelButtons()
        {
            Button btnRetry = new Button();
            btnRetry.Text = "OK";
            btnRetry.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "キャンセル";
            btnCancel.Click += ButtonClick;


            _buttonCollection.Add(btnRetry);
            _buttonCollection.Add(btnCancel);
        }

        private void InitYesNoButtons()
        {
            Button btnYes = new Button();
            btnYes.Text = "Yes";
            btnYes.Click += ButtonClick;

            Button btnNo = new Button();
            btnNo.Text = "No";
            btnNo.Click += ButtonClick;


            _buttonCollection.Add(btnYes);
            _buttonCollection.Add(btnNo);
        }

        private void InitYesNoCancelButtons()
        {
            Button btnYes = new Button();
            btnYes.Text = "Abort";
            btnYes.Click += ButtonClick;

            Button btnNo = new Button();
            btnNo.Text = "Retry";
            btnNo.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "キャンセル";
            btnCancel.Click += ButtonClick;

            _buttonCollection.Add(btnYes);
            _buttonCollection.Add(btnNo);
            _buttonCollection.Add(btnCancel);
        }

        private static void ButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Text)
            {
                case "Abort":
                    _buttonResult = DialogResult.Abort;
                    break;

                case "Retry":
                    _buttonResult = DialogResult.Retry;
                    break;

                case "Ignore":
                    _buttonResult = DialogResult.Ignore;
                    break;

                case "OK":
                    _buttonResult = DialogResult.OK;
                    break;

                case "キャンセル":
                    _buttonResult = DialogResult.Cancel;
                    break;

                case "Yes":
                    _buttonResult = DialogResult.Yes;
                    break;

                case "No":
                    _buttonResult = DialogResult.No;
                    break;
            }

            _msgBox.Dispose();
        }

        private static Size MessageSize(string message)
        {
            Graphics g = _msgBox.CreateGraphics();
            int width = 350;
            int height = 230;

            SizeF size = g.MeasureString(message, _msgBox._lblMessage.Font);

            if (message.Length < 150)
            {
                if ((int)size.Width > 350)
                {
                    width = (int)size.Width;
                }
            }
            else
            {
                string[] groups = (from Match m in Regex.Matches(message, ".{1,180}") select m.Value).ToArray();
                int lines = groups.Length + 1;
                width = 700;
                height += (int)(size.Height + 10) * lines;
            }
            return new Size(width, height);
        }
    }

    public enum MsgBoxButtons
    {
        AbortRetryIgnore = 1,
        OK = 2,
        OKCancel = 3,
        RetryCancel = 4,
        YesNo = 5,
        YesNoCancel = 6
    }

    public enum MsgBoxIcon
    {
        Application = 1,
        Exclamation = 2,
        Error = 3,
        Warning = 4,
        Info = 5,
        Question = 6,
        Shield = 7,
        Search = 8
    }
}
