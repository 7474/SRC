using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRCTestForm
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
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
                    try
                    {
                        SetStatusText($"Load data [{fbd.SelectedPath}].");
                        // TODO
                        SetStatusText("Loaded.");
                    }
                    catch (Exception ex)
                    {
                        SetStatusText(ex.Message);
                    }
                }
            }
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
    }
}
