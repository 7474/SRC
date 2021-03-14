using Microsoft.Extensions.Logging;
using SRCCore.Filesystem;
using SRCSharpForm.Resoruces;
using System;
using System.Windows.Forms;

namespace SRCSharpForm
{
    public partial class SRCSharpForm : Form
    {
        private SRCCore.SRC SRC;

        public SRCSharpForm()
        {
            InitializeComponent();

            SRC = new SRCCore.SRC();
            SRC.FileSystem = new LocalFileSystem();
            SRC.Sound.Player = new WindowsManagedPlayer();

            SRC.GUI = new SRCSharpFormGUI(SRC);
        }

        private void LoadEve()
        {
            using (var fbd = new OpenFileDialog())
            {
                fbd.Filter = "event files (*.eve)|*.eve|save files (*.src)|*.src";

                var res = fbd.ShowDialog();
                if (res == DialogResult.OK)
                {
                    Hide();
                    SRC.Execute(fbd.FileName);
                }
            }
        }

        private void SRCSharpForm_Shown(object sender, EventArgs e)
        {
            LoadEve();
        }
    }
}
