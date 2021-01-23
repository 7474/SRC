using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SRC.Core;

namespace SRCTestForm
{
    public partial class frmMain : Form, IGUI
    {
        public SRC.Core.SRC SRC;

        public frmMain()
        {
            InitializeComponent();

            SRC = new SRC.Core.SRC();
            SRC.GUI = this;
        }

        private void menuLoadData_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var res = fbd.ShowDialog();
                if (res == DialogResult.OK)
                {
                    //try
                    //{
                        SetStatusText($"Load data [{fbd.SelectedPath}].");
                        SRC.LoadDirectory(fbd.SelectedPath);
                        SetStatusText("Loaded.");
                    //}
                    //catch (Exception ex)
                    //{
                    //    SetStatusText(ex.Message);
                    //    SetMainText(ex.ToString());
                    //}
                }
            }
        }

        private void SetMainText(string text)
        {
            textMain.Text = text;
        }

        private void SetStatusText(string text)
        {
            toolStripStatusLabel.Text = text;
        }

        private void SetProgress(int value, int max)
        {
            toolStripProgressBar.Value = value;
            toolStripProgressBar.Maximum = max;
        }

        void IGUI.DisplayLoadingProgress()
        {
        }

        void IGUI.ErrorMessage(string str)
        {
            SetStatusText(str);
        }
    }
}
