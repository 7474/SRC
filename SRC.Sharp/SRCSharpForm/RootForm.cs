using Microsoft.Extensions.Logging;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCSharpForm.Resoruces;
using System;
using System.Windows.Forms;

namespace SRCSharpForm
{
    public partial class RootForm : Form
    {
        private SRCCore.SRC SRC;

        public RootForm()
        {
            InitializeComponent();

            SRC = new SRCCore.SRC();
            var fileSystem = new LocalFileSystem();
            try
            {
                fileSystem.AddAchive("assets.zip");
            }
            catch
            {
                // ignore
                // Assetsはお試し中
            }
            SRC.FileSystem = fileSystem;
            SRC.Sound.Player = new WindowsManagedPlayer();
            var config = new LocalFileConfig();
            SRC.SystemConfig = config;
            try
            {
                SRC.SystemConfig.Load();
            }
            catch
            {
                SRC.SystemConfig.Save();
            }
            // TODO 設定の反映処理を設ける
            // XXX 単位変更をこんな感じでやるのは下策だなー
            SRC.Sound.Player.SoundVolume = config.SoundVolume / 100f;

            SRC.GUI = new SRCSharpFormGUI(SRC);
            SRC.GUI.MessageWait = 700;
        }

        private void LoadGameFile()
        {
            using (var fbd = new OpenFileDialog())
            {
                fbd.Filter = "SRC# files (*.eve;*.srcs;*srcsq)|*.eve;*.srcs;*.srcq|event files (*.eve)|*.eve|save files (*.srcs;*srcsq)|*.srcs;*.srcq";

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
            // TODO 引数を参照して指定があればそれを読む。
            LoadGameFile();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            LoadGameFile();
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            SRC.GUI.Configure();
        }
    }
}
